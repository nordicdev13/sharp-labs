using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Admin.Cars;

public class DeleteModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly IWebHostEnvironment _env;

    public DeleteModel(RazorPagesCarContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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

            // Видаляємо папку із завантаженими фото видаленого автомобіля
            TryDeleteCarFolder(car.Id);
        }

        return RedirectToPage("./Index");
    }

    private void TryDeleteCarFolder(int carId)
    {
        var dir = Path.Combine(_env.WebRootPath, "uploads", "cars", carId.ToString());
        if (Directory.Exists(dir))
        {
            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch
            {
                // Ігноруємо можливі помилки видалення файлу
            }
        }
    }
}
