using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingSystem.Pages.Customers;

public class EditModel : PageModel
{
    private readonly HotelDbContext _context;

    public EditModel(HotelDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        Customer = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = await _context.Customers.FindAsync(Customer.Id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.FullName = Customer.FullName;
        existing.PassportNo = Customer.PassportNo;
        existing.NicNo = Customer.NicNo;
        existing.Phone = Customer.Phone;
        existing.Email = Customer.Email;
        existing.Country = Customer.Country;
        existing.Address = Customer.Address;

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
