using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Cars;

public class CreateModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public CreateModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Car Car { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Car.Add(Car);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
