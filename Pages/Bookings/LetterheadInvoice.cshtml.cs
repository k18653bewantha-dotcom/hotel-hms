using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Bookings;

[Authorize(Policy = "AdminOnly")]
public class LetterheadInvoiceModel : PageModel
{
    private readonly HotelDbContext _context;

    public LetterheadInvoiceModel(HotelDbContext context)
    {
        _context = context;
    }

    public Booking Booking { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        Booking = booking;
        return Page();
    }
}
