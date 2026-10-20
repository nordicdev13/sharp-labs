using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesCar.Data;
using RazorPagesCar.Models;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesCar.Pages.Contact;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly RazorPagesCarContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(RazorPagesCarContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string SupportEmail => "support@carcatalog.ua";
    public string Phone => "+380 (44) 123-45-67";

    public class ContactInput
    {
        [Required(ErrorMessage = "Будь ласка, вкажіть ваше ім'я"), StringLength(80)]
        [Display(Name = "Ваше ім’я")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, вкажіть адресу ел. пошти"), EmailAddress(ErrorMessage = "Некоректний формат email"), StringLength(120)]
        [Display(Name = "Ел. пошта")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть тему звернення"), StringLength(120)]
        [Display(Name = "Тема")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть текст повідомлення"), StringLength(4000)]
        [Display(Name = "Повідомлення")]
        public string Message { get; set; } = string.Empty;

        // Honeypot (поле має залишатися порожнім для реальних користувачів)
        public string? Website { get; set; }
    }

    [BindProperty]
    public ContactInput Input { get; set; } = new();

    [TempData]
    public string? OkMessage { get; set; }

    public async Task OnGetAsync()
    {
        // Автозаповнення даних для авторизованих користувачів
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Input.Email = user.Email ?? Input.Email;
                Input.Name = user.UserName ?? Input.Name;
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Простий антиспам (honeypot): якщо бот заповнив приховане поле — ігноруємо
        if (!string.IsNullOrWhiteSpace(Input.Website))
        {
            OkMessage = "Дякуємо, ваше повідомлення прийнято.";
            return RedirectToPage();
        }

        var feedback = new Feedback
        {
            Name = Input.Name.Trim(),
            Email = Input.Email.Trim(),
            Subject = Input.Subject.Trim(),
            Message = Input.Message.Trim(),
            UserId = User.Identity?.IsAuthenticated == true
                ? _userManager.GetUserId(User)
                : null
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        OkMessage = "Дякуємо! Ваше звернення успішно надіслано адміністрації.";
        return RedirectToPage();
    }
}
