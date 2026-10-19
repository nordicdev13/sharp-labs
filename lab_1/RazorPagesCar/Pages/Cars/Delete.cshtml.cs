using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Cars;

public class DeleteModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public DeleteModel(RazorPagesCarContext context)
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
        else
        {
            Car = car;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car.FindAsync(id);
        if (car != null)
        {
            Car = car;
            _context.Car.Remove(Car);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
