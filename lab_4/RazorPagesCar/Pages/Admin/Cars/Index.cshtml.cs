using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Admin.Cars;

public class IndexModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public IndexModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    public IList<Car> Car { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    public SelectList? Makes { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CarMake { get; set; }

    public async Task OnGetAsync()
    {
        // LINQ-запит для отримання унікальних відсортованих марок авто для випадаючого списку
        IQueryable<string> makeQuery = _context.Car
                                               .Select(c => c.Make)
                                               .Distinct()
                                               .OrderBy(m => m);

        // LINQ-запит для вибірки автомобілів
        var cars = from c in _context.Car
                   select c;

        // Пошук за рядком (модель або марка)
        if (!string.IsNullOrEmpty(SearchString))
        {
            cars = cars.Where(s => s.Model.Contains(SearchString) || s.Make.Contains(SearchString));
        }

        // Фільтрація за вибраною маркою
        if (!string.IsNullOrEmpty(CarMake))
        {
            cars = cars.Where(x => x.Make == CarMake);
        }

        // Побудова випадаючого списку без повторів
        Makes = new SelectList(await makeQuery.ToListAsync());

        // Виконання запиту та завантаження відфільтрованого списку
        Car = await cars.ToListAsync();
    }
}
