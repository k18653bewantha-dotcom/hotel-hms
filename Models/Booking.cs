using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models;

public class Booking
{
    public int Id { get; set; }

    [StringLength(30)]
    public string BookingNo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Select a customer.")]
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    [Required, StringLength(10)]
    public string MealPlan { get; set; } = MealPlans.RO;

    [Range(1, 1000)]
    public int Adults { get; set; } = 1;

    [Range(0, 1000)]
    public int Children { get; set; } = 0;

    [Required, StringLength(30)]
    public string Status { get; set; } = BookingStatuses.Reserved;

    [Required, StringLength(30)]
    public string PaymentStatus { get; set; } = PaymentStatuses.Pending;

    [StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    public decimal RoomCharge { get; set; }
    public decimal RestaurantBill { get; set; }
    public decimal BarBill { get; set; }
    public decimal LaundryBill { get; set; }
    public decimal SeasonalChargePercent { get; set; }
    public decimal SeasonalChargeAmount { get; set; }
    public decimal Discount { get; set; }

    // Deposit / advance received before final settlement.
    [Range(0, 1000000000)]
    public decimal AdvancePayment { get; set; }

    // GrandTotal is the invoice total before deducting the advance already paid.
    public decimal GrandTotal { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    public decimal ServiceTotal => RestaurantBill + BarBill + LaundryBill;

    [NotMapped]
    public decimal BalanceDue => PaymentStatus == PaymentStatuses.Paid
        ? 0
        : Math.Max(0, GrandTotal - AdvancePayment);

    [NotMapped]
    public int TotalGuests => Adults + Children;

    [NotMapped]
    public string RoomsDisplay => string.Join(", ",
        BookingRooms
            .Where(br => br.Room != null)
            .Select(br => br.Room!.RoomNumber)
            .OrderBy(x => x));
}

public static class BookingStatuses
{
    public const string Reserved = "Reserved";
    public const string CheckedIn = "CheckedIn";
    public const string CheckedOut = "CheckedOut";
    public const string Cancelled = "Cancelled";
}

public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string PartPaid = "Part Paid";
    public const string Paid = "Paid";
}

public static class MealPlans
{
    public const string RO = "RO";
    public const string BB = "BB";
    public const string HB = "HB";
    public const string FB = "FB";

    public static readonly string[] All = [RO, BB, HB, FB];
}
