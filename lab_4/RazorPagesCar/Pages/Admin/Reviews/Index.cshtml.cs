using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

namespace RazorPagesCar.Pages.Admin.Reviews;

public class IndexModel : PageModel
{
    private readonly RazorPagesCarContext _context;

    public IndexModel(RazorPagesCarContext context)
    {
        _context = context;
    }

    public record RowVM(
        int ReviewId,
        string CarTitle,
        string ReviewerName,
        int Rating,
        string? Comment,
        DateTime CreatedAt,
        string? AdminReply,
        DateTime? AdminRepliedAt
    );

    public IList<RowVM> Items { get; private set; } = new List<RowVM>();

    [BindProperty(SupportsGet = true)]
    public string? q { get; set; } // пошук за авто/автором/коментарем

    [BindProperty(SupportsGet = true)]
    public bool onlyUnreplied { get; set; } // лише без відповіді

    public async Task OnGetAsync()
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Include(r => r.Car)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q}%";
            query = query.Where(r =>
                EF.Functions.Like(r.Car!.Make, like) ||
                EF.Functions.Like(r.Car!.Model, like) ||
                EF.Functions.Like(r.ReviewerName!, like) ||
                EF.Functions.Like(r.Comment!, like));
        }

        if (onlyUnreplied)
        {
            query = query.Where(r => r.AdminReply == null || r.AdminReply == "");
        }

        Items = await query
            .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
            .Select(r => new RowVM(
                r.Id,
                r.Car != null ? $"{r.Car.Make} {r.Car.Model}" : "Авто",
                r.ReviewerName ?? "Користувач",
                r.Rating,
                r.Comment,
                r.CreatedAt,
                r.AdminReply,
                r.AdminRepliedAt
            ))
            .ToListAsync();
    }
}
