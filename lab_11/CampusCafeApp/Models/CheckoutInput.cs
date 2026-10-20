using System.ComponentModel.DataAnnotations;

namespace CampusCafe.Models;

public enum FulfillmentKind { Pickup, Delivery }

public class CheckoutInput : IValidatableObject
{
    [Required(ErrorMessage = "Укажіть ім’я.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Ім’я має містити від 2 до 60 символів.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажіть телефон.")]
    [RegularExpression(@"^\+380[0-9]{9}$", ErrorMessage = "Формат телефону: +380 і дев’ять цифр, без пробілів.")]
    public string Phone { get; set; } = "";

    [EnumDataType(typeof(FulfillmentKind), ErrorMessage = "Оберіть допустимий спосіб отримання.")]
    public FulfillmentKind Fulfillment { get; set; } = FulfillmentKind.Pickup;

    [StringLength(200, ErrorMessage = "Адреса має містити не більш як 200 символів.")]
    public string Address { get; set; } = "";

    [StringLength(500, ErrorMessage = "Коментар має містити не більш як 500 символів.")]
    public string Comment { get; set; } = "";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((Name ?? "").Trim().Length < 2)
            yield return new ValidationResult("Укажіть щонайменше два символи імені без крайніх пробілів.", new[] { nameof(Name) });
        if (Fulfillment == FulfillmentKind.Delivery && (Address ?? "").Trim().Length < 5)
            yield return new ValidationResult("Для доставки вкажіть адресу щонайменше з 5 символів.", new[] { nameof(Address) });
    }
}
