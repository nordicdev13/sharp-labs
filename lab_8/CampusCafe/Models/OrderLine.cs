namespace CampusCafe.Models;

public class OrderLine
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int DrinkId { get; init; }
    public string DrinkName { get; init; } = "";
    public int Volume { get; init; }
    public string Milk { get; init; } = "";
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Total => UnitPrice * Quantity;
}
