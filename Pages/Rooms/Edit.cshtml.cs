using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Rooms;

public class EditModel : PageModel
{
    private readonly HotelDbContext _context;

    public EditModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Room Room { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        Room = room;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _context.Rooms.AnyAsync(r =>
                r.RoomNumber == Room.RoomNumber && r.Id != Room.Id))
        {
            ModelState.AddModelError("Room.RoomNumber", "This room number already exists.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = await _context.Rooms.FindAsync(Room.Id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.RoomNumber = Room.RoomNumber;
        existing.RoomType = Room.RoomType;
        existing.Floor = Room.Floor;
        existing.Capacity = Room.Capacity;
        existing.PricePerNight = Room.PricePerNight;
        existing.Status = Room.Status;
        existing.IsActive = Room.IsActive;

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
