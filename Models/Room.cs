using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class Room
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string RoomType { get; set; } = "Double";

    [Range(1, 20)]
    public int Capacity { get; set; } = 2;

    [Range(0, 10000000)]
    public decimal PricePerNight { get; set; }

    [Required, StringLength(30)]
    public string Status { get; set; } = RoomStatuses.Ready;

    [StringLength(30)]
    public string Floor { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
}

public static class RoomStatuses
{
    public const string Ready = "Ready";
    public const string Reserved = "Reserved";
    public const string Occupied = "Occupied";
    public const string Dirty = "Dirty";
    public const string Cleaning = "Cleaning";
    public const string Maintenance = "Maintenance";

    public static readonly string[] All =
    [
        Ready, Reserved, Occupied, Dirty, Cleaning, Maintenance
    ];
}
