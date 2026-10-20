using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;
using RazorPagesCar.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RazorPagesCar.Pages.Admin.Feedback;

public class ReplyModel : PageModel
{
    private readonly RazorPagesCarContext _ctx;
    private readonly IEmailSender _email;
    private readonly UserManager<IdentityUser> _users;

    public ReplyModel(RazorPagesCarContext ctx, IEmailSender email, UserManager<IdentityUser> users)
    {
        _ctx = ctx;
        _email = email;
        _users = users;
    }

    public Models.Feedback? Item { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int id { get; set; }

    public class ReplyInput
    {
        [Required(ErrorMessage = "Вкажіть тему листа"), StringLength(200)]
        [Display(Name = "Тема листа")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть текст відповіді"), StringLength(4000)]
        [Display(Name = "Текст відповіді")]
        public string Body { get; set; } = string.Empty;
    }

    [BindProperty]
    public ReplyInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        this.id = id;
        Item = await _ctx.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
        if (Item == null)
        {
            return NotFound();
        }

        // Префіл: якщо відповіді ще не було — підставляємо тему з Re:
        Input.Subject = Item.AdminReplySubject ?? $"Re: {Item.Subject}";
        Input.Body = Item.AdminReplyBody ?? $"Вітаю, {Item.Name}!\n\nДякуємо за ваше звернення.\n\nЗ повагою,\nСлужба підтримки каталогу авто";

        return Page();
    }

    public async Task<IActionResult> OnPostSendAsync()
    {
        var f = await _ctx.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);
        if (f == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Item = f;
            return Page();
        }

        // Безпечне формування HTML для тіла листа
        var safeName = WebUtility.HtmlEncode(f.Name);
        var safeSubject = WebUtility.HtmlEncode(f.Subject);
        var safeBody = WebUtility.HtmlEncode(Input.Body).Replace("\r\n", "\n").Replace("\n", "<br/>");

        var html = $"<p>Вітаю, {safeName}!</p><p>{safeBody}</p><hr/><p style='color:gray;font-size:small;'>Тема вашого первинного звернення: <b>{safeSubject}</b></p>";

        await _email.SendAsync(f.Email, Input.Subject, html, Input.Body);

        // Фіксуємо статус відповіді в базі даних
        var admin = await _users.GetUserAsync(User);
        f.AdminReplySubject = Input.Subject;
        f.AdminReplyBody = Input.Body;
        f.AdminRepliedBy = admin?.Email ?? admin?.UserName ?? "Адміністратор";
        f.AdminRepliedAt = DateTime.UtcNow;

        await _ctx.SaveChangesAsync();

        TempData["Ok"] = $"Відповідь на звернення користувача {f.Name} успішно надіслано.";
        return RedirectToPage("./Index");
    }
}
