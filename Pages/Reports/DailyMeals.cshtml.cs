using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Reports;

public class DailyMealsModel : PageModel
{
    private readonly HotelDbContext _context;

    public DailyMealsModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime Date { get; set; }

    public List<MealRoomRow> Rows { get; set; } = new();

    public int TotalRooms { get; set; }
    public int TotalGuests { get; set; }

    public int BBRooms { get; set; }
    public int HBRooms { get; set; }
    public int FBRooms { get; set; }

    public int BBPax { get; set; }
    public int HBPax { get; set; }
    public int FBPax { get; set; }

    public int BreakfastPax { get; set; }
    public int LunchPax { get; set; }
    public int DinnerPax { get; set; }

    public async Task OnGetAsync()
    {
        if (Date == default)
        {
            Date = DateTime.Today;
        }

        var day = Date.Date;
        var nextDay = day.AddDays(1);

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .Where(b =>
                b.Status != BookingStatuses.Cancelled &&
                b.CheckIn < nextDay &&
                b.CheckOut >= day)
            .OrderBy(b => b.CheckOut)
            .ThenBy(b => b.BookingNo)
            .ToListAsync();

        foreach (var booking in bookings)
        {
            var pax = Math.Max(0, booking.TotalGuests);
            var roomCount = booking.BookingRooms.Count(br => br.Room != null);
            var plan = MealPlans.All.Contains(booking.MealPlan)
                ? booking.MealPlan
                : MealPlans.RO;

            // Breakfast is for guests who stayed the previous night, including checkout day.
            var breakfast = plan != MealPlans.RO &&
                            booking.CheckIn.Date < day &&
                            booking.CheckOut.Date >= day
                ? pax
                : 0;

            // Full Board includes lunch while the guest remains in-house after the selected day starts.
            var lunch = plan == MealPlans.FB &&
                        booking.CheckIn.Date <= day &&
                        booking.CheckOut.Date > day
                ? pax
                : 0;

            // Half Board is treated as breakfast + dinner. Full Board includes dinner as well.
            var dinner = (plan == MealPlans.HB || plan == MealPlans.FB) &&
                         booking.CheckIn.Date <= day &&
                         booking.CheckOut.Date > day
                ? pax
                : 0;

            Rows.Add(new MealRoomRow
            {
                BookingNo = booking.BookingNo,
                Rooms = booking.RoomsDisplay,
                GuestName = booking.Customer?.FullName ?? string.Empty,
                MealPlan = plan,
                Adults = booking.Adults,
                Children = booking.Children,
                TotalPax = pax,
                BreakfastPax = breakfast,
                LunchPax = lunch,
                DinnerPax = dinner,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                Status = booking.Status
            });

            TotalRooms += roomCount;
            TotalGuests += pax;

            if (plan == MealPlans.BB)
            {
                BBRooms += roomCount;
                BBPax += pax;
            }
            else if (plan == MealPlans.HB)
            {
                HBRooms += roomCount;
                HBPax += pax;
            }
            else if (plan == MealPlans.FB)
            {
                FBRooms += roomCount;
                FBPax += pax;
            }
        }

        BreakfastPax = Rows.Sum(r => r.BreakfastPax);
        LunchPax = Rows.Sum(r => r.LunchPax);
        DinnerPax = Rows.Sum(r => r.DinnerPax);
    }

    public class MealRoomRow
    {
        public string BookingNo { get; set; } = string.Empty;
        public string Rooms { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string MealPlan { get; set; } = MealPlans.RO;
        public int Adults { get; set; }
        public int Children { get; set; }
        public int TotalPax { get; set; }
        public int BreakfastPax { get; set; }
        public int LunchPax { get; set; }
        public int DinnerPax { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
