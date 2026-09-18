using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public IndexModel(
        HotelDbContext context,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public List<UserAccount> Users { get; set; } = new();

    [BindProperty]
    public CreateUserInput NewUser { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadUsersAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        ValidatePassword(NewUser.TemporaryPassword, "NewUser.TemporaryPassword");

        if (!UserRoles.All.Contains(NewUser.Role))
        {
            ModelState.AddModelError("NewUser.Role", "Select a valid role.");
        }

        var username = NewUser.Username.Trim().ToLowerInvariant();
        if (await _context.UserAccounts.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError("NewUser.Username", "This username already exists.");
        }

        if (!ModelState.IsValid)
        {
            await LoadUsersAsync();
            return Page();
        }

        var user = new UserAccount
        {
            Username = username,
            DisplayName = NewUser.DisplayName.Trim(),
            Role = NewUser.Role,
            IsActive = true,
            MustChangePassword = true,
            CreatedAt = DateTime.Now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, NewUser.TemporaryPassword);

        _context.UserAccounts.Add(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Account '{user.Username}' created. The user must change the temporary password after login.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var currentId = GetCurrentUserId();
        if (user.Id == currentId)
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToPage();
        }

        if (user.IsActive && user.Role == UserRoles.Admin)
        {
            var activeAdminCount = await _context.UserAccounts.CountAsync(u =>
                u.IsActive && u.Role == UserRoles.Admin);

            if (activeAdminCount <= 1)
            {
                TempData["Error"] = "At least one active administrator account is required.";
                return RedirectToPage();
            }
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"{user.Username} is now {(user.IsActive ? "active" : "inactive")}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRoleAsync(int id, string role)
    {
        if (!UserRoles.All.Contains(role))
        {
            return BadRequest();
        }

        var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var currentId = GetCurrentUserId();
        if (user.Id == currentId && role != UserRoles.Admin)
        {
            TempData["Error"] = "You cannot remove your own administrator role.";
            return RedirectToPage();
        }

        if (user.Role == UserRoles.Admin && role != UserRoles.Admin && user.IsActive)
        {
            var activeAdminCount = await _context.UserAccounts.CountAsync(u =>
                u.IsActive && u.Role == UserRoles.Admin);

            if (activeAdminCount <= 1)
            {
                TempData["Error"] = "At least one active administrator account is required.";
                return RedirectToPage();
            }
        }

        user.Role = role;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Role updated for {user.Username}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostResetPasswordAsync(int id, string temporaryPassword)
    {
        if (!IsStrongPassword(temporaryPassword))
        {
            TempData["Error"] = "Temporary password must contain at least 8 characters, one letter and one number.";
            return RedirectToPage();
        }

        var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == GetCurrentUserId())
        {
            TempData["Error"] = "Use Change Password to update your own password.";
            return RedirectToPage();
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, temporaryPassword);
        user.MustChangePassword = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Password reset for {user.Username}. The user must change it after login.";
        return RedirectToPage();
    }

    private async Task LoadUsersAsync()
    {
        Users = await _context.UserAccounts
            .AsNoTracking()
            .OrderBy(u => u.Role)
            .ThenBy(u => u.DisplayName)
            .ToListAsync();
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : 0;
    }

    private void ValidatePassword(string password, string field)
    {
        if (!IsStrongPassword(password))
        {
            ModelState.AddModelError(
                field,
                "Use at least 8 characters with at least one letter and one number.");
        }
    }

    private static bool IsStrongPassword(string? password) =>
        !string.IsNullOrWhiteSpace(password) &&
        password.Length >= 8 &&
        password.Any(char.IsLetter) &&
        password.Any(char.IsDigit);

    public class CreateUserInput
    {
        [Required, StringLength(100)]
        [Display(Name = "Full name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = UserRoles.Reception;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Temporary password")]
        public string TemporaryPassword { get; set; } = string.Empty;
    }
}
