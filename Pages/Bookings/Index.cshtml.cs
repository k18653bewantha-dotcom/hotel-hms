using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly BookingBillingService _billing;

    public IndexModel(
        HotelDbContext context,
        BookingBillingService billing)
    {
        _context = context;
        _billing = billing;
    }

    public List<Booking> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        Bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .OrderByDescending(b => b.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCheckInAsync(int id)
    {
        var booking = await FindBookingAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        if (booking.Status != BookingStatuses.Reserved)
        {
            TempData["Error"] = "Only reserved bookings can be checked in.";
            return RedirectToPage();
        }

        booking.Status = BookingStatuses.CheckedIn;

        foreach (var bookingRoom in booking.BookingRooms)
        {
            if (bookingRoom.Room != null)
            {
                bookingRoom.Room.Status = RoomStatuses.Occupied;
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = $"{booking.BookingNo} checked in successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync(
        int id,
        decimal barBill,
        decimal laundryBill,
        decimal seasonalChargePercent,
        decimal discount,
        string? paymentStatus,
        string? paymentMethod,
        string? remarks)
    {
        var booking = await FindBookingAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        if (booking.Status != BookingStatuses.CheckedIn)
        {
            TempData["Error"] = "Only checked-in bookings can be checked out.";
            return RedirectToPage();
        }

        var hasPendingRoomOrders = await _context.Orders.AnyAsync(o =>
            o.BookingId == booking.Id &&
            o.OrderType == OrderTypes.RoomCharge &&
            o.Status != OrderStatuses.Served &&
            o.Status != OrderStatuses.Cancelled);

        if (hasPendingRoomOrders)
        {
            TempData["Error"] =
                "Complete or cancel all pending restaurant room-charge orders before checkout.";
            return RedirectToPage();
        }

        string[] allowedPaymentStatuses =
        [
            PaymentStatuses.Pending,
            PaymentStatuses.PartPaid,
            PaymentStatuses.Paid
        ];

        string[] allowedPaymentMethods =
        [
            "Cash",
            "Credit Card",
            "Bank Transfer",
            "Online Payment"
        ];

        // Only Admin may change invoice/billing values before checkout.
        // Reception may complete checkout, but posted financial values are ignored.
        if (User.IsInRole(UserRoles.Admin))
        {
            // RestaurantBill is maintained automatically by served room-charge orders.
            booking.BarBill = Math.Max(0, barBill);
            booking.LaundryBill = Math.Max(0, laundryBill);
            booking.SeasonalChargePercent = Math.Max(0, seasonalChargePercent);
            booking.Discount = Math.Max(0, discount);
            booking.PaymentStatus = paymentStatus != null && allowedPaymentStatuses.Contains(paymentStatus)
                ? paymentStatus
                : booking.PaymentStatus;

            booking.PaymentMethod = paymentMethod != null && allowedPaymentMethods.Contains(paymentMethod)
                ? paymentMethod
                : booking.PaymentMethod;
            booking.Remarks = remarks ?? booking.Remarks;
        }

        booking.Status = BookingStatuses.CheckedOut;
        _billing.Recalculate(booking);

        foreach (var bookingRoom in booking.BookingRooms)
        {
            if (bookingRoom.Room != null)
            {
                bookingRoom.Room.Status = RoomStatuses.Ready;
            }
        }

        await _context.SaveChangesAsync();

        if (User.IsInRole(UserRoles.Admin))
        {
            return RedirectToPage("Invoice", new { id = booking.Id });
        }

        TempData["Success"] = $"{booking.BookingNo} checked out successfully. Invoice printing is restricted to Admin.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var booking = await FindBookingAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        if (booking.Status == BookingStatuses.CheckedIn)
        {
            TempData["Error"] = "Checked-in bookings cannot be cancelled.";
            return RedirectToPage();
        }

        booking.Status = BookingStatuses.Cancelled;

        foreach (var bookingRoom in booking.BookingRooms)
        {
            if (bookingRoom.Room != null &&
                bookingRoom.Room.Status == RoomStatuses.Reserved)
            {
                bookingRoom.Room.Status = RoomStatuses.Ready;
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = $"{booking.BookingNo} was cancelled.";
        return RedirectToPage();
    }

    private Task<Booking?> FindBookingAsync(int id)
    {
        return _context.Bookings
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
