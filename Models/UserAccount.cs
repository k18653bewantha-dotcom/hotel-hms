using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models;

public class UserAccount
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Role { get; set; } = UserRoles.Reception;

    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLoginAt { get; set; }
}

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Reception = "Reception";
    public const string Restaurant = "Restaurant";
    public const string Housekeeping = "Housekeeping";

    public static readonly string[] All =
    [
        Admin,
        Reception,
        Restaurant,
        Housekeeping
    ];
}
