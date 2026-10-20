using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Catalog;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public IndexModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    public IList<Car> Cars { get; private set; } = new List<Car>();

    [BindProperty(SupportsGet = true)]
    public string? q { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? make { get; set; }

    public SelectList MakeOptions { get; private set; } = default!;

    public async Task OnGetAsync()
    {
        // 1) Отримуємо унікальні відсортовані марки
        var makes = await _context.Car.AsNoTracking()
            .Where(c => !string.IsNullOrEmpty(c.Make))
            .Select(c => c.Make)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();

        MakeOptions = new SelectList(makes);

        // 2) Базовий запит з AsNoTracking для оптимізації читання
        var query = _context.Car.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var pattern = $"%{q}%";
            query = query.Where(c => EF.Functions.Like(c.Model, pattern) || EF.Functions.Like(c.Make, pattern));
        }

        if (!string.IsNullOrWhiteSpace(make))
        {
            query = query.Where(c => c.Make == make);
        }

        Cars = await query
            .OrderBy(c => c.Make)
            .ThenBy(c => c.Model)
            .ToListAsync();
    }
}
