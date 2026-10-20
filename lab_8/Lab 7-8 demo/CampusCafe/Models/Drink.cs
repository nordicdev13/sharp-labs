namespace CampusCafe.Models;

public class Drink
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "☕";
    public decimal BasePrice { get; set; }
}
