using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesCar.Models;

public class Car
{
    public int Id { get; set; }

    [Display(Name = "Марка")]
    [Required(ErrorMessage = "Поле 'Марка' є обов'язковим")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва марки повинна містити від 2 до 50 символів")]
    public string Make { get; set; } = string.Empty;

    [Display(Name = "Модель")]
    [Required(ErrorMessage = "Поле 'Модель' є обов'язковим")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Назва моделі повинна містити від 1 до 50 символів")]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Рік випуску")]
    [Range(1900, 2026, ErrorMessage = "Вкажіть коректний рік випуску (від 1900 до 2026)")]
    public int Year { get; set; }

    [Display(Name = "Ціна ($)")]
    [Range(1, 10_000_000, ErrorMessage = "Ціна повинна бути більшою за 0")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Display(Name = "Колір")]
    [StringLength(30, ErrorMessage = "Назва кольору не повинна перевищувати 30 символів")]
    public string? Color { get; set; }
}
