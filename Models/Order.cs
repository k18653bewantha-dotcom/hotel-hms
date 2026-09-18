using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class Order
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string OrderNumber { get; set; } = string.Empty;

    public int? RestaurantTableId { get; set; }
    public RestaurantTable? RestaurantTable { get; set; }

    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }

    [Required, StringLength(30)]
    public string OrderType { get; set; } = OrderTypes.DineIn;

    [Required, StringLength(30)]
    public string Status { get; set; } = OrderStatuses.Pending;

    public DateTime OrderTime { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    [StringLength(300)]
    public string Notes { get; set; } = string.Empty;

    public bool PostedToRoom { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public static class OrderStatuses
{
    public const string Pending = "Pending";
    public const string Preparing = "Preparing";
    public const string Ready = "Ready";
    public const string Served = "Served";
    public const string Cancelled = "Cancelled";
}

public static class OrderTypes
{
    public const string DineIn = "DineIn";
    public const string RoomCharge = "RoomCharge";
}
