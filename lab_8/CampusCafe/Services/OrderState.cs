using CampusCafe.Models;

namespace CampusCafe.Services;

public class OrderState
{
    private readonly List<OrderLine> items = new();
    public IReadOnlyList<OrderLine> Items => items.AsReadOnly();
    public decimal Total => items.Sum(item => item.Total);
    public int Count => items.Sum(item => item.Quantity);
    public event Action? Changed;

    public void Add(OrderLine item)
    {
        if (item.Quantity is < 1 or > 10 || item.UnitPrice < 0)
            throw new ArgumentException("Некоректна позиція замовлення.");
        items.Add(item);
        Changed?.Invoke();
    }

    public void Remove(Guid id)
    {
        items.RemoveAll(item => item.Id == id);
        Changed?.Invoke();
    }

    public void Clear()
    {
        items.Clear();
        Changed?.Invoke();
    }
}
