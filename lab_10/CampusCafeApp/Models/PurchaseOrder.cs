using System.ComponentModel.DataAnnotations.Schema;

namespace CampusCafe.Models;

public class PurchaseOrder
{
    public int Id { get; set; }
    public Guid RequestId { get; set; }
    public DateTime CreatedUtc { get; set; }
    // У лабораторній 9 ці поля порожні; форма з'явиться в лабораторній 10.
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string Comment { get; set; } = "";
    public List<PurchaseItem> Items { get; set; } = new();
    [NotMapped] public decimal Total => Items.Sum(item => item.Total);
}

public class PurchaseItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public string ProductName { get; set; } = "";
    public string Options { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [NotMapped] public decimal Total => UnitPrice * Quantity;
}
