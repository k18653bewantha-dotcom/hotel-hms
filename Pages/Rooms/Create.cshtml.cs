using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Rooms;

public class CreateModel : PageModel
{
    private readonly HotelDbContext _context;

    public CreateModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Room Room { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _context.Rooms.AnyAsync(r => r.RoomNumber == Room.RoomNumber))
        {
            ModelState.AddModelError("Room.RoomNumber", "This room number already exists.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Rooms.Add(Room);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
