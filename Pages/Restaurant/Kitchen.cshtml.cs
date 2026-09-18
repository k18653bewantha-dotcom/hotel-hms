using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Restaurant;

public class KitchenModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly RestaurantChargeService _restaurantCharges;

    public KitchenModel(
        HotelDbContext context,
        RestaurantChargeService restaurantCharges)
    {
        _context = context;
        _restaurantCharges = restaurantCharges;
    }

    public List<Order> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        Orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.RestaurantTable)
            .Include(o => o.Booking)
                .ThenInclude(b => b!.BookingRooms)
                    .ThenInclude(br => br.Room)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Where(o =>
                o.Status != OrderStatuses.Served &&
                o.Status != OrderStatuses.Cancelled)
            .OrderBy(o => o.OrderTime)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostStatusAsync(int id, string status)
    {
        var order = await _context.Orders
            .Include(o => o.RestaurantTable)
            .Include(o => o.Booking)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        if (!IsAllowedTransition(order.Status, status))
        {
            TempData["Error"] = $"Order cannot move from {order.Status} to {status}.";
            return RedirectToPage();
        }

        _restaurantCharges.ApplyStatusChange(order, status);

        if (status is OrderStatuses.Served or OrderStatuses.Cancelled)
        {
            if (order.RestaurantTable != null)
            {
                order.RestaurantTable.Status = "Available";
            }
        }

        await _context.SaveChangesAsync();

        if (status == OrderStatuses.Served &&
            order.OrderType == OrderTypes.RoomCharge &&
            order.Booking != null)
        {
            TempData["Success"] =
                $"{order.OrderNumber} served. Rs. {order.TotalAmount:N2} was added to {order.Booking.BookingNo}.";
        }

        return RedirectToPage();
    }

    private static bool IsAllowedTransition(string current, string next)
    {
        if (next == OrderStatuses.Cancelled)
        {
            return current is OrderStatuses.Pending or OrderStatuses.Preparing or OrderStatuses.Ready;
        }

        return (current, next) switch
        {
            (OrderStatuses.Pending, OrderStatuses.Preparing) => true,
            (OrderStatuses.Preparing, OrderStatuses.Ready) => true,
            (OrderStatuses.Ready, OrderStatuses.Served) => true,
            _ => false
        };
    }
}
