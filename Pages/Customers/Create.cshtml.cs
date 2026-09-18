using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingSystem.Pages.Customers;

public class CreateModel : PageModel
{
    private readonly HotelDbContext _context;

    public CreateModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Customers.Add(Customer);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
