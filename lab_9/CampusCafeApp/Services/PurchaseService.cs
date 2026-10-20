using CampusCafe.Data;
using CampusCafe.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCafe.Services;

public class PurchaseService(
    IDbContextFactory<OrderDbContext> factory,
    CatalogService catalog)
{
    public async Task<int> PlaceAsync(IReadOnlyList<OrderLine> lines, Guid requestId)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Відсутній ідентифікатор операції.");
        await using var db = await factory.CreateDbContextAsync();
        var existing = await db.Orders.SingleOrDefaultAsync(o => o.RequestId == requestId);
        if (existing is not null) return existing.Id;
        if (lines.Count is 0 or > 50)
            throw new ArgumentException("Замовлення має містити від 1 до 50 позицій.");

        var products = (await catalog.GetDrinksAsync()).ToDictionary(p => p.Id);
        var order = new PurchaseOrder { RequestId = requestId, CreatedUtc = DateTime.UtcNow };
        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.DrinkId, out var product))
                throw new ArgumentException("Товар більше недоступний.");
            var draft = new DrinkDraft
            {
                Volume = line.Volume,
                Quantity = line.Quantity,
                Milk = line.Milk switch
                {
                    "звичайне" => MilkKind.Regular,
                    "вівсяне" => MilkKind.Oat,
                    _ => throw new ArgumentException("Невідомий вид молока.")
                }
            };
            var checkedLine = draft.CreateLine(product);
            // Не приймаємо підмінену або застарілу ціну без згоди користувача.
            if (checkedLine.UnitPrice != line.UnitPrice)
                throw new ArgumentException("Ціна змінилася. Видаліть позицію й додайте її з каталогу знову.");
            order.Items.Add(new PurchaseItem
            {
                ProductName = product.Name,
                Options = $"{checkedLine.Volume} мл · {checkedLine.Milk} молоко",
                Quantity = checkedLine.Quantity,
                UnitPrice = checkedLine.UnitPrice
            });
        }
        db.Orders.Add(order);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            // Повтор одночасного запиту з тим самим ключем не створює дубль.
            await using var retry = await factory.CreateDbContextAsync();
            var saved = await retry.Orders.AsNoTracking()
                .SingleOrDefaultAsync(o => o.RequestId == requestId);
            if (saved is not null) return saved.Id;
            throw;
        }
        return order.Id;
    }

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Orders.AsNoTracking().Include(o => o.Items)
            .OrderByDescending(o => o.Id).ToListAsync();
    }

    public async Task<PurchaseOrder?> GetAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Orders.AsNoTracking().Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);
    }
}
