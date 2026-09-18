using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Housekeeping;

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
            .Where(r => r.IsActive)
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
}
