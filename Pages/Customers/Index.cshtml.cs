using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<Customer> Customers { get; set; } = new();

    public async Task OnGetAsync(string? search)
    {
        var query = _context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.FullName.Contains(search) ||
                c.Phone.Contains(search) ||
                c.PassportNo.Contains(search) ||
                c.NicNo.Contains(search));
        }

        Customers = await query.OrderBy(c => c.FullName).ToListAsync();
    }
}
