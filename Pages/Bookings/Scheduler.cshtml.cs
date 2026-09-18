using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Bookings;

public class SchedulerModel : PageModel
{
    private readonly HotelDbContext _context;

    public SchedulerModel(HotelDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }

    public async Task<JsonResult> OnGetEventsAsync()
    {
        var bookingRooms = await _context.BookingRooms
            .AsNoTracking()
            .Include(br => br.Room)
            .Include(br => br.Booking)
                .ThenInclude(b => b!.Customer)
            .Where(br =>
                br.Booking != null &&
                br.Booking.Status != BookingStatuses.Cancelled)
            .ToListAsync();

        var isAdmin = User.IsInRole(UserRoles.Admin);

        var events = bookingRooms.Select(br => new
        {
            id = $"{br.BookingId}-{br.RoomId}",
            title = $"Room {br.Room?.RoomNumber} · {br.Booking?.Customer?.FullName}",
            start = br.Booking!.CheckIn,
            end = br.Booking.CheckOut,
            color = GetRoomColor(br.RoomId),
            url = isAdmin ? $"/Bookings/Invoice/{br.BookingId}" : "/Bookings",
            extendedProps = new
            {
                bookingNo = br.Booking.BookingNo,
                room = br.Room?.RoomNumber,
                status = br.Booking.Status
            }
        });

        return new JsonResult(events);
    }

    private static string GetRoomColor(int roomId)
    {
        string[] colors =
        [
            "#0d6efd", "#198754", "#dc3545", "#6f42c1",
            "#fd7e14", "#20c997", "#6610f2", "#d63384",
            "#0dcaf0", "#795548", "#607d8b", "#8bc34a"
        ];

        return colors[Math.Abs(roomId) % colors.Length];
    }
}
