using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Rooms;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<Room> Rooms { get; set; } = new();

    public async Task OnGetAsync()
    {
        Rooms = await _context.Rooms
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(int id, string status)
    {
        if (!RoomStatuses.All.Contains(status))
        {
            return BadRequest();
        }

        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        room.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        room.IsActive = !room.IsActive;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
