using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Cars;

public class EditModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public EditModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Car Car { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car.FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }
        Car = car;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Car).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(Car.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool CarExists(int id)
    {
        return _context.Car.Any(e => e.Id == id);
    }
}
