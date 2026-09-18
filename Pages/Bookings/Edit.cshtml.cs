using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Bookings;

[Authorize(Policy = "AdminOnly")]
public class EditModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly RoomAvailabilityService _availability;
    private readonly BookingBillingService _billing;

    public EditModel(
        HotelDbContext context,
        RoomAvailabilityService availability,
        BookingBillingService billing)
    {
        _context = context;
        _availability = availability;
        _billing = billing;
    }

    [BindProperty]
    public EditBookingInput Input { get; set; } = new();

    [BindProperty]
    public List<int> SelectedRoomIds { get; set; } = new();

    public SelectList Customers { get; set; } = default!;
    public List<Room> AvailableRooms { get; set; } = new();
    public string BookingNo { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public decimal RestaurantBill { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        if (!CanEdit(booking))
        {
            TempData["Error"] = "Only bookings before checkout can be edited.";
            return RedirectToPage("Index");
        }

        Input = new EditBookingInput
        {
            Id = booking.Id,
            CustomerId = booking.CustomerId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            MealPlan = booking.MealPlan,
            Adults = booking.Adults,
            Children = booking.Children,
            RoomCharge = booking.RoomCharge,
            BarBill = booking.BarBill,
            LaundryBill = booking.LaundryBill,
            SeasonalChargePercent = booking.SeasonalChargePercent,
            Discount = booking.Discount,
            AdvancePayment = booking.AdvancePayment,
            PaymentStatus = booking.PaymentStatus,
            PaymentMethod = booking.PaymentMethod,
            Remarks = booking.Remarks
        };

        SelectedRoomIds = booking.BookingRooms.Select(br => br.RoomId).ToList();
        BookingNo = booking.BookingNo;
        BookingStatus = booking.Status;
        RestaurantBill = booking.RestaurantBill;

        await LoadPageDataAsync(id, booking.CheckIn, booking.CheckOut);
        return Page();
    }

    public async Task<JsonResult> OnGetAvailableRoomsAsync(
        int id,
        DateTime checkIn,
        DateTime checkOut)
    {
        var rooms = await _availability.GetAvailableRoomsAsync(
            checkIn,
            checkOut,
            id);

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
        var booking = await _context.Bookings
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .FirstOrDefaultAsync(b => b.Id == Input.Id);

        if (booking == null)
        {
            return NotFound();
        }

        BookingNo = booking.BookingNo;
        BookingStatus = booking.Status;
        RestaurantBill = booking.RestaurantBill;

        if (!CanEdit(booking))
        {
            TempData["Error"] = "This booking can no longer be edited because it has already been checked out or cancelled.";
            return RedirectToPage("Index");
        }

        SelectedRoomIds = SelectedRoomIds.Distinct().ToList();

        if (Input.CheckOut <= Input.CheckIn)
        {
            ModelState.AddModelError(string.Empty, "Check-out must be after check-in.");
        }

        if (!MealPlans.All.Contains(Input.MealPlan))
        {
            ModelState.AddModelError(nameof(Input.MealPlan), "Select a valid meal plan.");
        }

        if (SelectedRoomIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one room.");
        }

        if (ModelState.IsValid)
        {
            var expectedSeasonal = Math.Max(0, Input.RoomCharge)
                * Math.Max(0, Input.SeasonalChargePercent) / 100m;
            var expectedGrandTotal = Math.Max(0,
                Math.Max(0, Input.RoomCharge)
                + expectedSeasonal
                + Math.Max(0, booking.RestaurantBill)
                + Math.Max(0, Input.BarBill)
                + Math.Max(0, Input.LaundryBill)
                - Math.Max(0, Input.Discount));

            if (Input.AdvancePayment > expectedGrandTotal)
            {
                ModelState.AddModelError(nameof(Input.AdvancePayment),
                    "Advance payment cannot be greater than the current booking total.");
            }
        }

        if (ModelState.IsValid)
        {
            var roomsAvailable = await _availability.AreRoomsAvailableAsync(
                SelectedRoomIds,
                Input.CheckIn,
                Input.CheckOut,
                booking.Id);

            if (!roomsAvailable)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "One or more selected rooms are not available for the new dates.");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync(booking.Id, Input.CheckIn, Input.CheckOut);
            return Page();
        }

        var allowedPaymentStatuses = new[]
        {
            PaymentStatuses.Pending,
            PaymentStatuses.PartPaid,
            PaymentStatuses.Paid
        };

        var allowedPaymentMethods = new[]
        {
            "Cash",
            "Credit Card",
            "Bank Transfer",
            "Online Payment"
        };

        var selectedSet = SelectedRoomIds.ToHashSet();
        var existingSet = booking.BookingRooms.Select(br => br.RoomId).ToHashSet();

        var removedBookingRooms = booking.BookingRooms
            .Where(br => !selectedSet.Contains(br.RoomId))
            .ToList();

        foreach (var removed in removedBookingRooms)
        {
            if (removed.Room != null)
            {
                if (booking.Status == BookingStatuses.CheckedIn &&
                    removed.Room.Status == RoomStatuses.Occupied)
                {
                    removed.Room.Status = RoomStatuses.Ready;
                }
                else if (removed.Room.Status == RoomStatuses.Reserved)
                {
                    removed.Room.Status = RoomStatuses.Ready;
                }
            }
        }

        _context.BookingRooms.RemoveRange(removedBookingRooms);

        var addedIds = SelectedRoomIds.Where(id => !existingSet.Contains(id)).ToList();
        var addedRooms = await _context.Rooms
            .Where(r => addedIds.Contains(r.Id))
            .ToListAsync();

        foreach (var room in addedRooms)
        {
            booking.BookingRooms.Add(new BookingRoom
            {
                BookingId = booking.Id,
                RoomId = room.Id,
                Room = room
            });

            if (booking.Status == BookingStatuses.CheckedIn)
            {
                room.Status = RoomStatuses.Occupied;
            }
            else if (Input.CheckIn <= DateTime.Now.AddDays(1))
            {
                room.Status = RoomStatuses.Reserved;
            }
        }

        foreach (var existing in booking.BookingRooms
                     .Where(br => selectedSet.Contains(br.RoomId) && br.Room != null))
        {
            if (booking.Status == BookingStatuses.CheckedIn)
            {
                existing.Room!.Status = RoomStatuses.Occupied;
            }
            else if (Input.CheckIn <= DateTime.Now.AddDays(1))
            {
                existing.Room!.Status = RoomStatuses.Reserved;
            }
            else if (existing.Room!.Status == RoomStatuses.Reserved)
            {
                existing.Room.Status = RoomStatuses.Ready;
            }
        }

        booking.CustomerId = Input.CustomerId;
        booking.CheckIn = Input.CheckIn;
        booking.CheckOut = Input.CheckOut;
        booking.MealPlan = Input.MealPlan;
        booking.Adults = Input.Adults;
        booking.Children = Input.Children;
        booking.RoomCharge = Input.RoomCharge;
        booking.BarBill = Input.BarBill;
        booking.LaundryBill = Input.LaundryBill;
        booking.SeasonalChargePercent = Input.SeasonalChargePercent;
        booking.Discount = Input.Discount;
        booking.AdvancePayment = Input.AdvancePayment;
        booking.PaymentStatus = allowedPaymentStatuses.Contains(Input.PaymentStatus)
            ? Input.PaymentStatus
            : PaymentStatuses.Pending;
        booking.PaymentMethod = allowedPaymentMethods.Contains(Input.PaymentMethod)
            ? Input.PaymentMethod
            : "Cash";
        booking.Remarks = Input.Remarks ?? string.Empty;

        // RestaurantBill is intentionally not manually editable.
        // It remains controlled by served Restaurant POS room-charge orders.
        _billing.Recalculate(booking);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{booking.BookingNo} updated successfully by Admin.";
        return RedirectToPage("Index");
    }

    private async Task LoadPageDataAsync(
        int bookingId,
        DateTime checkIn,
        DateTime checkOut)
    {
        Customers = new SelectList(
            await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.FullName)
                .ToListAsync(),
            "Id",
            "FullName",
            Input.CustomerId);

        AvailableRooms = checkOut > checkIn
            ? await _availability.GetAvailableRoomsAsync(checkIn, checkOut, bookingId)
            : new List<Room>();
    }

    private static bool CanEdit(Booking booking) =>
        booking.Status != BookingStatuses.CheckedOut &&
        booking.Status != BookingStatuses.Cancelled;

    public class EditBookingInput
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a customer.")]
        public int CustomerId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        [Required, StringLength(10)]
        public string MealPlan { get; set; } = MealPlans.RO;

        [Range(1, 1000)]
        public int Adults { get; set; } = 1;

        [Range(0, 1000)]
        public int Children { get; set; }

        [Range(0, 1000000000)]
        public decimal RoomCharge { get; set; }

        [Range(0, 1000000000)]
        public decimal BarBill { get; set; }

        [Range(0, 1000000000)]
        public decimal LaundryBill { get; set; }

        [Range(0, 1000)]
        public decimal SeasonalChargePercent { get; set; }

        [Range(0, 1000000000)]
        public decimal Discount { get; set; }

        [Range(0, 1000000000)]
        public decimal AdvancePayment { get; set; }

        [Required, StringLength(30)]
        public string PaymentStatus { get; set; } = PaymentStatuses.Pending;

        [Required, StringLength(30)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
