using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public int TotalRooms { get; set; }
    public int ReadyRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int MaintenanceRooms { get; set; }
    public int TodayArrivals { get; set; }
    public int TodayDepartures { get; set; }
    public int InHouseGuests { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal TodayRestaurantSales { get; set; }

    public List<Booking> UpcomingBookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        TotalRooms = await _context.Rooms.CountAsync(r => r.IsActive);
        ReadyRooms = await _context.Rooms.CountAsync(r => r.IsActive && r.Status == RoomStatuses.Ready);
        OccupiedRooms = await _context.Rooms.CountAsync(r => r.IsActive && r.Status == RoomStatuses.Occupied);
        MaintenanceRooms = await _context.Rooms.CountAsync(r => r.IsActive && r.Status == RoomStatuses.Maintenance);

        TodayArrivals = await _context.Bookings.CountAsync(b =>
            b.Status != BookingStatuses.Cancelled &&
            b.CheckIn >= today &&
            b.CheckIn < tomorrow);

        TodayDepartures = await _context.Bookings.CountAsync(b =>
            b.Status != BookingStatuses.Cancelled &&
            b.CheckOut >= today &&
            b.CheckOut < tomorrow);

        InHouseGuests = await _context.Bookings.CountAsync(b => b.Status == BookingStatuses.CheckedIn);

        TodayRevenue = await _context.Bookings
            .Where(b => b.Status == BookingStatuses.CheckedOut &&
                        b.CheckOut >= today &&
                        b.CheckOut < tomorrow)
            .SumAsync(b => (decimal?)b.GrandTotal) ?? 0;

        TodayRestaurantSales = await _context.Orders
            .Where(o => o.Status != OrderStatuses.Cancelled &&
                        o.OrderTime >= today &&
                        o.OrderTime < tomorrow)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        UpcomingBookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .Where(b => b.Status == BookingStatuses.Reserved &&
                        b.CheckIn >= today)
            .OrderBy(b => b.CheckIn)
            .Take(8)
            .ToListAsync();
    }
}
