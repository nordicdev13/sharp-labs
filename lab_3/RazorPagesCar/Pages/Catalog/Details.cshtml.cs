using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Catalog;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DetailsModel(RazorPagesCarContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Car? Car { get; private set; }
    public IList<Review> Reviews { get; private set; } = new List<Review>();
    public double AverageRating { get; private set; }
    public int ReviewsCount => Reviews.Count;

    [BindProperty(SupportsGet = true)]
    public int id { get; set; }

    [BindProperty]
    public int? MyRating { get; set; }

    [BindProperty]
    public string? MyComment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        this.id = id;
        Car = await _context.Car.AsNoTracking()
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Car == null)
        {
            return NotFound();
        }

        Reviews = Car.Reviews
            .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
            .ToList();

        AverageRating = Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;

        // Якщо користувач авторизований, завантажуємо його попередній відгук у форму
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User);
            var mine = Reviews.FirstOrDefault(r => r.UserId == userId);
            if (mine != null)
            {
                MyRating = mine.Rating;
                MyComment = mine.Comment;
            }
        }

        return Page();
    }

    // Збереження або оновлення відгуку
    public async Task<IActionResult> OnPostSaveReviewAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (id <= 0)
        {
            return NotFound();
        }

        if (MyRating is null || MyRating < 1 || MyRating > 5)
        {
            ModelState.AddModelError(nameof(MyRating), "Оцінка повинна бути від 1 до 5.");
            await OnGetAsync(id);
            return Page();
        }

        var car = await _context.Car.FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var userId = user.Id;
        var displayName = user.Email ?? user.UserName ?? "Користувач";

        var existing = await _context.Reviews.FirstOrDefaultAsync(r => r.CarId == id && r.UserId == userId);

        if (existing == null)
        {
            _context.Reviews.Add(new Review
            {
                CarId = id,
                UserId = userId,
                ReviewerName = displayName,
                Rating = MyRating.Value,
                Comment = string.IsNullOrWhiteSpace(MyComment) ? null : MyComment.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Rating = MyRating.Value;
            existing.Comment = string.IsNullOrWhiteSpace(MyComment) ? null : MyComment.Trim();
            existing.ReviewerName = displayName;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Details", new { id });
    }

    // Видалення відгуку (власником або адміністратором)
    public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        if (review == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var currentUserId = user?.Id;
        var isOwner = currentUserId == review.UserId;
        var isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin)
        {
            return Forbid();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Details", new { id = review.CarId });
    }
}
