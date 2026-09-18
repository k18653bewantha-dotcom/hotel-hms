using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class MenuItem
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
    public int MenuCategoryId { get; set; }
    public MenuCategory? MenuCategory { get; set; }

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;
}
