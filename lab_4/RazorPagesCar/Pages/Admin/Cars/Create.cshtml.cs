using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Admin.Cars;

public class CreateModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateModel(RazorPagesCarContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Car Car { get; set; } = default!;

    [BindProperty]
    public IFormFile? PosterFile { get; set; }

    private static readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long _maxFileSize = 2 * 1024 * 1024; // 2 МБ

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Car.Add(Car);
        await _context.SaveChangesAsync();

        // Якщо завантажено файл — перевіряємо, зберігаємо та оновлюємо PosterPath
        if (PosterFile is not null && PosterFile.Length > 0)
        {
            try
            {
                Car.PosterPath = await SavePosterAsync(PosterFile, Car.Id);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Помилка завантаження фото: {ex.Message}");
                return Page();
            }
        }

        return RedirectToPage("./Index");
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
            throw new InvalidOperationException("Файл порожній або перевищує максимально допустимий розмір 2 МБ.");
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
}
