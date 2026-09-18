using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "KingswoodHotelPMS.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.SlidingExpiration = true;

        options.Events.OnValidatePrincipal = async context =>
        {
            var idValue = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idValue, out var userId))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            await using var scope = context.HttpContext.RequestServices.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            var account = await db.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (account == null || !account.IsActive)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            var currentRole = context.Principal?.FindFirst(ClaimTypes.Role)?.Value;
            var currentName = context.Principal?.Identity?.Name;
            var currentMustChange = context.Principal?
                .FindFirst("must_change_password")?.Value == "true";

            if (currentRole != account.Role ||
                currentName != account.DisplayName ||
                currentMustChange != account.MustChangePassword)
            {
                Claim[] claims =
                [
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Name, account.DisplayName),
                    new(ClaimTypes.Role, account.Role),
                    new("username", account.Username),
                    new("must_change_password",
                        account.MustChangePassword ? "true" : "false")
                ];

                context.ReplacePrincipal(new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme)));
                context.ShouldRenew = true;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRoles.Admin));

    options.AddPolicy("ReceptionAccess", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.Reception));

    options.AddPolicy("RestaurantAccess", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.Restaurant));

    options.AddPolicy("HousekeepingAccess", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.Housekeeping));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Setup");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Error");

    options.Conventions.AuthorizePage("/Index", "ReceptionAccess");
    options.Conventions.AuthorizeFolder("/Bookings", "ReceptionAccess");
    options.Conventions.AuthorizePage("/Bookings/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/Bookings/Invoice", "AdminOnly");
    options.Conventions.AuthorizePage("/Bookings/LetterheadInvoice", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Rooms", "ReceptionAccess");
    options.Conventions.AuthorizeFolder("/Customers", "ReceptionAccess");
    options.Conventions.AuthorizeFolder("/Reports", "ReceptionAccess");
    options.Conventions.AuthorizeFolder("/Restaurant", "RestaurantAccess");
    options.Conventions.AuthorizeFolder("/Housekeeping", "HousekeepingAccess");
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

var configuredDatabasePath = builder.Configuration["DatabasePath"];
var databasePath = string.IsNullOrWhiteSpace(configuredDatabasePath)
    ? Path.Combine(builder.Environment.ContentRootPath, "hotel.db")
    : Path.GetFullPath(configuredDatabasePath);

var databaseDirectory = Path.GetDirectoryName(databasePath);
if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
builder.Services.AddScoped<RoomAvailabilityService>();
builder.Services.AddScoped<BookingBillingService>();
builder.Services.AddScoped<RestaurantChargeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

app.Use(async (context, next) =>
{
    var mustChangePassword = context.User.FindFirst("must_change_password")?.Value == "true";
    var path = context.Request.Path;

    if (context.User.Identity?.IsAuthenticated == true &&
        mustChangePassword &&
        !path.StartsWithSegments("/Account/ChangePassword") &&
        !path.StartsWithSegments("/Account/Logout") &&
        !path.StartsWithSegments("/Account/AccessDenied"))
    {
        context.Response.Redirect("/Account/ChangePassword");
        return;
    }

    await next();
});

app.UseAuthorization();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    context.Database.EnsureCreated();
    DatabaseSchemaUpgrade.Apply(context);
    DbSeeder.Seed(context);
}

app.Run();
