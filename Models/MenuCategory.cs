using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class MenuCategory
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
