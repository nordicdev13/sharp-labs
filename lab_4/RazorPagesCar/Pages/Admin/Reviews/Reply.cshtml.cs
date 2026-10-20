using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesCar.Pages.Admin.Reviews;

public class ReplyModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ReplyModel(RazorPagesCarContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Review? Review { get; private set; }
    public string CarTitle { get; private set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int id { get; set; }

    public class ReplyInput
    {
        [Display(Name = "Офіційна відповідь адміністратора")]
        [StringLength(2000, ErrorMessage = "Відповідь не повинна перевищувати 2000 символів")]
        public string? AdminReply { get; set; }
    }

    [BindProperty]
    public ReplyInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        this.id = id;
        Review = await _context.Reviews
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (Review == null)
        {
            return NotFound();
        }

        CarTitle = Review.Car != null ? $"{Review.Car.Make} {Review.Car.Model}" : "Автомобіль";
        Input.AdminReply = Review.AdminReply;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var review = await _context.Reviews
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Review = review;
            CarTitle = review.Car != null ? $"{review.Car.Make} {review.Car.Model}" : "Автомобіль";
            return Page();
        }

        var text = string.IsNullOrWhiteSpace(Input.AdminReply) ? null : Input.AdminReply.Trim();
        var admin = await _userManager.GetUserAsync(User);
        var adminName = admin?.Email ?? admin?.UserName ?? "Адміністратор";

        review.AdminReply = text;
        review.AdminReplyBy = text == null ? null : adminName;
        review.AdminRepliedAt = text == null ? (DateTime?)null : DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
