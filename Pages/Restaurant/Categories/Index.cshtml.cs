using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Pages.Restaurant.Categories;

public class IndexModel : PageModel
{
    private readonly HotelDbContext _context;

    public IndexModel(HotelDbContext context)
    {
        _context = context;
    }

    public List<MenuCategory> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _context.MenuCategories
            .Include(c => c.MenuItems)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync(string name)
    {
        name = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Category name is required.";
            return RedirectToPage();
        }

        if (await _context.MenuCategories.AnyAsync(c => c.Name == name))
        {
            TempData["Error"] = "This category already exists.";
            return RedirectToPage();
        }

        _context.MenuCategories.Add(new MenuCategory { Name = name });
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var category = await _context.MenuCategories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.IsActive = !category.IsActive;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var category = await _context.MenuCategories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        if (category.MenuItems.Count > 0)
        {
            TempData["Error"] = "Remove or move menu items before deleting this category.";
            return RedirectToPage();
        }

        _context.MenuCategories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
