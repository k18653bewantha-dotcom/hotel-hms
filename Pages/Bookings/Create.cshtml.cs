using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Bookings;

public class CreateModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly RoomAvailabilityService _availability;
    private readonly BookingBillingService _billing;

    public CreateModel(
        HotelDbContext context,
        RoomAvailabilityService availability,
        BookingBillingService billing)
    {
        _context = context;
        _availability = availability;
        _billing = billing;
    }

    [BindProperty]
    public Booking Booking { get; set; } = new();

    [BindProperty]
    public List<int> SelectedRoomIds { get; set; } = new();

    public SelectList Customers { get; set; } = default!;
    public List<Room> AvailableRooms { get; set; } = new();

    public async Task OnGetAsync()
    {
        Booking.CheckIn = DateTime.Today.AddDays(1).AddHours(14);
        Booking.CheckOut = DateTime.Today.AddDays(2).AddHours(12);
        Booking.PaymentStatus = PaymentStatuses.Pending;
        Booking.PaymentMethod = "Cash";
        Booking.MealPlan = MealPlans.RO;
        Booking.Adults = 1;
        Booking.Children = 0;
        Booking.AdvancePayment = 0;

        await LoadPageDataAsync();
    }

    public async Task<JsonResult> OnGetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        var rooms = await _availability.GetAvailableRoomsAsync(checkIn, checkOut);

        return new JsonResult(rooms.Select(r => new
        {
            id = r.Id,
            roomNumber = r.RoomNumber,
            roomType = r.RoomType,
            capacity = r.Capacity,
            status = r.Status
        }));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadPageDataAsync();

        if (Booking.CheckOut <= Booking.CheckIn)
        {
            ModelState.AddModelError(string.Empty, "Check-out must be after check-in.");
        }

        if (!MealPlans.All.Contains(Booking.MealPlan))
        {
            ModelState.AddModelError(nameof(Booking.MealPlan), "Select a valid meal plan.");
        }

        if (Booking.Adults < 1)
        {
            ModelState.AddModelError(nameof(Booking.Adults), "At least one adult is required.");
        }

        if (Booking.Children < 0)
        {
            ModelState.AddModelError(nameof(Booking.Children), "Children cannot be negative.");
        }

        SelectedRoomIds = SelectedRoomIds.Distinct().ToList();

        if (SelectedRoomIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one available room.");
        }

        _billing.Recalculate(Booking);

        if (Booking.AdvancePayment > Booking.GrandTotal)
        {
            ModelState.AddModelError(nameof(Booking.AdvancePayment),
                "Advance payment cannot be greater than the current booking total.");
        }

        if (Booking.AdvancePayment > 0 && Booking.PaymentStatus == PaymentStatuses.Pending)
        {
            Booking.PaymentStatus = Booking.AdvancePayment >= Booking.GrandTotal && Booking.GrandTotal > 0
                ? PaymentStatuses.Paid
                : PaymentStatuses.PartPaid;
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var stillAvailable = await _availability.AreRoomsAvailableAsync(
            SelectedRoomIds,
            Booking.CheckIn,
            Booking.CheckOut);

        if (!stillAvailable)
        {
            ModelState.AddModelError(
                string.Empty,
                "One or more selected rooms have just been booked. Please select rooms again.");

            await LoadPageDataAsync();
            return Page();
        }

        Booking.BookingNo = $"TMP-{Guid.NewGuid().ToString("N")[..12]}";
        Booking.Status = BookingStatuses.Reserved;
        Booking.CreatedAt = DateTime.Now;
        Booking.RestaurantBill = 0;

        _billing.Recalculate(Booking);

        _context.Bookings.Add(Booking);
        await _context.SaveChangesAsync();

        Booking.BookingNo = $"BK-{Booking.Id:00000}";

        foreach (var roomId in SelectedRoomIds)
        {
            _context.BookingRooms.Add(new BookingRoom
            {
                BookingId = Booking.Id,
                RoomId = roomId
            });
        }

        var rooms = await _context.Rooms
            .Where(r => SelectedRoomIds.Contains(r.Id))
            .ToListAsync();

        if (Booking.CheckIn <= DateTime.Now.AddDays(1))
        {
            foreach (var room in rooms)
            {
                room.Status = RoomStatuses.Reserved;
            }
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadPageDataAsync()
    {
        Customers = new SelectList(
            await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.FullName)
                .ToListAsync(),
            "Id",
            "FullName",
            Booking.CustomerId);

        if (Booking.CheckOut > Booking.CheckIn)
        {
            AvailableRooms = await _availability.GetAvailableRoomsAsync(
                Booking.CheckIn,
                Booking.CheckOut);
        }
        else
        {
            AvailableRooms = new List<Room>();
        }
    }
}
