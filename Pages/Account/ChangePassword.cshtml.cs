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

public class ChangePasswordModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public ChangePasswordModel(
        HotelDbContext context,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public PasswordInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateNewPassword();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var idValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idValue, out var userId))
        {
            return RedirectToPage("Login");
        }

        var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return RedirectToPage("Login");
        }

        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            Input.CurrentPassword);

        if (verification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("Input.CurrentPassword", "Current password is incorrect.");
            return Page();
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, Input.NewPassword);
        user.MustChangePassword = false;
        await _context.SaveChangesAsync();

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Password changed successfully. Sign in with your new password.";
        return RedirectToPage("Login");
    }

    private void ValidateNewPassword()
    {
        if (Input.NewPassword != Input.ConfirmPassword)
        {
            ModelState.AddModelError("Input.ConfirmPassword", "Passwords do not match.");
        }

        if (Input.NewPassword.Length < 8 ||
            !Input.NewPassword.Any(char.IsLetter) ||
            !Input.NewPassword.Any(char.IsDigit))
        {
            ModelState.AddModelError(
                "Input.NewPassword",
                "Use at least 8 characters with at least one letter and one number.");
        }
    }


    public class PasswordInput
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
