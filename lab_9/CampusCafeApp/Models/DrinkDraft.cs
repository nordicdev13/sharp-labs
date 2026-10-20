namespace CampusCafe.Models;

public enum MilkKind { Regular, Oat }

public class DrinkDraft
{
    public int Volume { get; set; } = 250;
    public MilkKind Milk { get; set; } = MilkKind.Regular;
    public int Quantity { get; set; } = 1;

    public decimal GetUnitPrice(Drink drink)
    {
        decimal volumeExtra = Volume switch
        {
            250 => 0m,
            350 => 15m,
            450 => 25m,
            _ => throw new InvalidOperationException("Неприпустимий об’єм.")
        };
        decimal milkExtra = Milk switch
        {
            MilkKind.Regular => 0m,
            MilkKind.Oat => 20m,
            _ => throw new InvalidOperationException("Неприпустимий вид молока.")
        };
        return drink.BasePrice + volumeExtra + milkExtra;
    }

    public OrderLine CreateLine(Drink drink)
    {
        if (Quantity is < 1 or > 10)
            throw new InvalidOperationException("Кількість має бути від 1 до 10.");

        return new OrderLine
        {
            DrinkId = drink.Id,
            DrinkName = drink.Name,
            Volume = Volume,
            Milk = Milk == MilkKind.Oat ? "вівсяне" : "звичайне",
            Quantity = Quantity,
            UnitPrice = GetUnitPrice(drink)
        };
    }
}
