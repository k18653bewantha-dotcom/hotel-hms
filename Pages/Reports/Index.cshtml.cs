using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Reports;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime From { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime To { get; set; }

    public decimal HotelRevenue { get; set; }
    public decimal RestaurantSales { get; set; }
    public decimal RoomRevenue { get; set; }
    public decimal ServiceRevenue { get; set; }
    public int CheckedOutBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int TotalRoomNights { get; set; }

    public List<Booking> Bookings { get; set; } = new();
    public List<Order> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (From == default)
        {
            From = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        if (To == default)
        {
            To = DateTime.Today;
        }

        var from = From.Date;
        var until = To.Date.AddDays(1);

        Bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .Where(b =>
                b.CheckOut >= from &&
                b.CheckOut < until)
            .OrderByDescending(b => b.CheckOut)
            .ToListAsync();

        Orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.RestaurantTable)
            .Include(o => o.Booking)
            .Where(o =>
                o.OrderTime >= from &&
                o.OrderTime < until &&
                o.Status != OrderStatuses.Cancelled)
            .OrderByDescending(o => o.OrderTime)
            .ToListAsync();

        var checkedOut = Bookings
            .Where(b => b.Status == BookingStatuses.CheckedOut)
            .ToList();

        HotelRevenue = checkedOut.Sum(b => b.GrandTotal);
        RoomRevenue = checkedOut.Sum(b => b.RoomCharge + b.SeasonalChargeAmount);
        ServiceRevenue = checkedOut.Sum(b => b.ServiceTotal);
        RestaurantSales = Orders.Sum(o => o.TotalAmount);
        CheckedOutBookings = checkedOut.Count;
        CancelledBookings = Bookings.Count(b => b.Status == BookingStatuses.Cancelled);
        TotalRoomNights = checkedOut.Sum(b =>
            Math.Max(1, (b.CheckOut.Date - b.CheckIn.Date).Days) *
            b.BookingRooms.Count);
    }
}
