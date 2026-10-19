using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Cars;

public class IndexModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public IndexModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    public IList<Car> Car { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Car = await _context.Car.ToListAsync();
    }
}
