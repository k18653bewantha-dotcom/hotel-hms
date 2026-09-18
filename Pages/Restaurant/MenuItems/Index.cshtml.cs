using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Restaurant.MenuItems;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<MenuItem> MenuItems { get; set; } = new();
    public SelectList Categories { get; set; } = default!;

    [BindProperty]
    public MenuItem NewItem { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        _context.MenuItems.Add(NewItem);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(
        int id,
        string name,
        int menuCategoryId,
        decimal price)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        item.Name = name.Trim();
        item.MenuCategoryId = menuCategoryId;
        item.Price = Math.Max(0, price);

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        item.IsAvailable = !item.IsAvailable;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var used = await _context.OrderItems.AnyAsync(oi => oi.MenuItemId == id);
        if (used)
        {
            TempData["Error"] = "This menu item has order history. Disable it instead of deleting it.";
            return RedirectToPage();
        }

        var item = await _context.MenuItems.FindAsync(id);
        if (item != null)
        {
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        MenuItems = await _context.MenuItems
            .Include(m => m.MenuCategory)
            .OrderBy(m => m.MenuCategory!.Name)
            .ThenBy(m => m.Name)
            .ToListAsync();

        Categories = new SelectList(
            await _context.MenuCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "Id",
            "Name");
    }
}
