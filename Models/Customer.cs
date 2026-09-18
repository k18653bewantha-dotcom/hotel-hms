using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string PassportNo { get; set; } = string.Empty;

    [StringLength(50)]
    public string NicNo { get; set; } = string.Empty;

    [StringLength(40)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(80)]
    public string Country { get; set; } = "Sri Lanka";

    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
