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

public class LoginModel : PageModel
{
    private readonly HotelDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public LoginModel(
        HotelDbContext context,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _context.UserAccounts.AnyAsync())
        {
            return RedirectToPage("Setup");
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return Redirect(GetHomeUrl(User.FindFirst(ClaimTypes.Role)?.Value));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _context.UserAccounts.AnyAsync())
        {
            return RedirectToPage("Setup");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var username = NormalizeUsername(Input.Username);
        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            Input.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);
        }

        user.LastLoginAt = DateTime.Now;
        await _context.SaveChangesAsync();
        await SignInAsync(user, Input.RememberMe);

        if (user.MustChangePassword)
        {
            return RedirectToPage("ChangePassword");
        }

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return Redirect(GetHomeUrl(user.Role));
    }

    private async Task SignInAsync(UserAccount user, bool rememberMe)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, user.Role),
            new("username", user.Username),
            new("must_change_password", user.MustChangePassword ? "true" : "false")
        ];

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(10),
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);
    }

    private static string NormalizeUsername(string username) =>
        username.Trim().ToLowerInvariant();

    private static string GetHomeUrl(string? role) => role switch
    {
        UserRoles.Restaurant => "/Restaurant/POS",
        UserRoles.Housekeeping => "/Housekeeping",
        _ => "/"
    };

    public class LoginInput
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
