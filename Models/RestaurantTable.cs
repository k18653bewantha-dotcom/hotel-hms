using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class RestaurantTable
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string TableNumber { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Status { get; set; } = "Available";

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
