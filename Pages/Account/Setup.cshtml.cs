using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Account;

public class SetupModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public SetupModel(
        HotelDbContext context,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public SetupInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (await _context.UserAccounts.AnyAsync())
        {
            return RedirectToPage("Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _context.UserAccounts.AnyAsync())
        {
            return RedirectToPage("Login");
        }

        ValidatePassword(Input.Password, Input.ConfirmPassword);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new UserAccount
        {
            Username = Input.Username.Trim().ToLowerInvariant(),
            DisplayName = Input.DisplayName.Trim(),
            Role = UserRoles.Admin,
            IsActive = true,
            MustChangePassword = false,
            CreatedAt = DateTime.Now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);

        _context.UserAccounts.Add(user);
        await _context.SaveChangesAsync();

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, user.Role),
            new("username", user.Username),
            new("must_change_password", "false")
        ];

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme)));

        return RedirectToPage("/Index");
    }

    private void ValidatePassword(string password, string confirmation)
    {
        if (password != confirmation)
        {
            ModelState.AddModelError("Input.ConfirmPassword", "Passwords do not match.");
        }

        if (password.Length < 8 ||
            !password.Any(char.IsLetter) ||
            !password.Any(char.IsDigit))
        {
            ModelState.AddModelError(
                "Input.Password",
                "Use at least 8 characters with at least one letter and one number.");
        }
    }

    public class SetupInput
    {
        [Required, StringLength(100)]
        [Display(Name = "Administrator name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
