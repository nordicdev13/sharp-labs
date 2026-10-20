using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Admin.Cars;

public class EditModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly IWebHostEnvironment _env;

    public EditModel(RazorPagesCarContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Car Car { get; set; } = default!;

    [BindProperty]
    public IFormFile? PosterFile { get; set; }

    private static readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long _maxFileSize = 2 * 1024 * 1024; // 2 МБ

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

        // Захист від overposting: оновлюємо дозволені властивості через завантажену сутність
        var dbCar = await _context.Car.FirstOrDefaultAsync(m => m.Id == Car.Id);
        if (dbCar == null)
        {
            return NotFound();
        }

        dbCar.Make = Car.Make;
        dbCar.Model = Car.Model;
        dbCar.Year = Car.Year;
        dbCar.Price = Car.Price;
        dbCar.BodyType = Car.BodyType;
        dbCar.Color = Car.Color;

        if (PosterFile is not null && PosterFile.Length > 0)
        {
            try
            {
                var newPath = await SavePosterAsync(PosterFile, dbCar.Id);
                // Видаляємо старий файл фото для чистоти файлової системи
                TryDeleteFile(dbCar.PosterPath);
                dbCar.PosterPath = newPath;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Помилка завантаження фото: {ex.Message}");
                return Page();
            }
        }

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

    private async Task<string> SavePosterAsync(IFormFile file, int carId)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_permittedExtensions.Contains(ext))
        {
            throw new InvalidOperationException("Неприпустимий формат зображення. Дозволені: jpg, jpeg, png, webp.");
        }

        if (file.Length <= 0 || file.Length > _maxFileSize)
        {
            throw new InvalidOperationException("Файл порожній або перевищує 2 МБ.");
        }

        var destDir = Path.Combine(_env.WebRootPath, "uploads", "cars", carId.ToString());
        Directory.CreateDirectory(destDir);

        var fileName = $"poster-{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(destDir, fileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/cars/{carId}/{fileName}";
    }

    private void TryDeleteFile(string? webPath)
    {
        if (string.IsNullOrWhiteSpace(webPath)) return;
        var fullPath = Path.Combine(_env.WebRootPath, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(fullPath))
        {
            try { System.IO.File.Delete(fullPath); } catch { }
        }
    }
}
