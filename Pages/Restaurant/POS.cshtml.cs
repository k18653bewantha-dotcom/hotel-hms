using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Restaurant;

public class POSModel : PageModel
{
    private readonly HotelDbContext _context;

    public POSModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<MenuCategory> Categories { get; set; } = new();
    public List<MenuItem> MenuItems { get; set; } = new();
    public List<RestaurantTable> Tables { get; set; } = new();
    public List<Booking> InHouseBookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _context.MenuCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        MenuItems = await _context.MenuItems
            .AsNoTracking()
            .Include(m => m.MenuCategory)
            .Where(m => m.IsAvailable && m.MenuCategory != null && m.MenuCategory.IsActive)
            .OrderBy(m => m.MenuCategory!.Name)
            .ThenBy(m => m.Name)
            .ToListAsync();

        Tables = await _context.RestaurantTables
            .AsNoTracking()
            .OrderBy(t => t.TableNumber)
            .ToListAsync();

        InHouseBookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .Where(b => b.Status == BookingStatuses.CheckedIn)
            .OrderBy(b => b.BookingNo)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostSendOrderAsync([FromBody] OrderRequest? request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid order request." });
        }

        request.OrderType = request.OrderType == OrderTypes.RoomCharge
            ? OrderTypes.RoomCharge
            : OrderTypes.DineIn;

        if (request.Items.Count == 0)
        {
            return BadRequest(new { message = "Select at least one menu item." });
        }

        var menuItemIds = request.Items
            .Select(i => i.MenuItemId)
            .Distinct()
            .ToList();

        var menuItems = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.Id) && m.IsAvailable)
            .ToDictionaryAsync(m => m.Id);

        if (menuItems.Count != menuItemIds.Count)
        {
            return BadRequest(new { message = "One or more menu items are unavailable." });
        }

        if (request.OrderType == OrderTypes.RoomCharge)
        {
            var validBooking = request.BookingId.HasValue &&
                await _context.Bookings.AnyAsync(b =>
                    b.Id == request.BookingId.Value &&
                    b.Status == BookingStatuses.CheckedIn);

            if (!validBooking)
            {
                return BadRequest(new { message = "Select a valid in-house room booking." });
            }
        }
        else
        {
            if (!request.RestaurantTableId.HasValue ||
                !await _context.RestaurantTables.AnyAsync(t => t.Id == request.RestaurantTableId.Value))
            {
                return BadRequest(new { message = "Select a restaurant table." });
            }
        }

        var order = new Order
        {
            OrderNumber = $"TMP-{Guid.NewGuid().ToString("N")[..12]}",
            RestaurantTableId = request.OrderType == OrderTypes.DineIn
                ? request.RestaurantTableId
                : null,
            BookingId = request.OrderType == OrderTypes.RoomCharge
                ? request.BookingId
                : null,
            OrderType = request.OrderType,
            Status = OrderStatuses.Pending,
            OrderTime = DateTime.Now,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        foreach (var requestedItem in request.Items)
        {
            if (!menuItems.TryGetValue(requestedItem.MenuItemId, out var menuItem))
            {
                continue;
            }

            var quantity = Math.Max(1, requestedItem.Quantity);

            order.OrderItems.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = quantity,
                UnitPrice = menuItem.Price,
                TotalPrice = menuItem.Price * quantity
            });
        }

        order.TotalAmount = order.OrderItems.Sum(i => i.TotalPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        order.OrderNumber = $"ORD-{order.Id:00000}";

        if (order.RestaurantTableId.HasValue)
        {
            var table = await _context.RestaurantTables.FindAsync(order.RestaurantTableId.Value);
            if (table != null)
            {
                table.Status = "Occupied";
            }
        }

        await _context.SaveChangesAsync();

        return new JsonResult(new
        {
            success = true,
            orderNumber = order.OrderNumber,
            total = order.TotalAmount
        });
    }

    public class OrderRequest
    {
        public string OrderType { get; set; } = OrderTypes.DineIn;
        public int? RestaurantTableId { get; set; }
        public int? BookingId { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
