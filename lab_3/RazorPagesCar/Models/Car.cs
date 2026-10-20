using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesCar.Models;

public class Car
{
    public int Id { get; set; }

    [Display(Name = "Марка")]
    [Required(ErrorMessage = "Поле 'Марка' є обов'язковим")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Марка повинна містити від 2 до 50 символів")]
    [RegularExpression(@"^[A-ZА-ЯІЇЄ][a-zA-Zа-яА-ЯіїєІЇЄ0-9\s-]*$", ErrorMessage = "Марка має починатися з великої літери")]
    public string Make { get; set; } = string.Empty;

    [Display(Name = "Модель")]
    [Required(ErrorMessage = "Поле 'Модель' є обов'язковим")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Модель повинна містити від 1 до 50 символів")]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Рік випуску")]
    [Range(1900, 2026, ErrorMessage = "Рік випуску повинен бути в діапазоні від 1900 до 2026")]
    public int Year { get; set; }

    [Display(Name = "Ціна ($)")]
    [Range(1, 10_000_000, ErrorMessage = "Ціна повинна бути більшою за 0")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Display(Name = "Тип кузова")]
    [Required(ErrorMessage = "Поле 'Тип кузова' є обов'язковим")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Тип кузова повинен містити від 3 до 30 символів")]
    [RegularExpression(@"^[A-ZА-ЯІЇЄ][a-zA-Zа-яА-ЯіїєІЇЄ\s-]*$", ErrorMessage = "Тип кузова має починатися з великої літери")]
    public string BodyType { get; set; } = string.Empty;

    [Display(Name = "Колір")]
    [StringLength(30, ErrorMessage = "Колір не повинен перевищувати 30 символів")]
    public string? Color { get; set; }

    [Display(Name = "Фото авто")]
    public string? PosterPath { get; set; } // Відносний шлях, наприклад /uploads/cars/1/poster.webp

    // Відгуки про авто
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
