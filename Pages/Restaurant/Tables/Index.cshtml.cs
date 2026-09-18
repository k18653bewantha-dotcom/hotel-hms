using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Restaurant.Tables;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<RestaurantTable> Tables { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tables = await _context.RestaurantTables
            .OrderBy(t => t.TableNumber)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync(string tableNumber)
    {
        tableNumber = (tableNumber ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tableNumber))
        {
            return RedirectToPage();
        }

        if (!await _context.RestaurantTables.AnyAsync(t => t.TableNumber == tableNumber))
        {
            _context.RestaurantTables.Add(new RestaurantTable
            {
                TableNumber = tableNumber,
                Status = "Available"
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(int id, string status)
    {
        var table = await _context.RestaurantTables.FindAsync(id);
        if (table == null)
        {
            return NotFound();
        }

        table.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
