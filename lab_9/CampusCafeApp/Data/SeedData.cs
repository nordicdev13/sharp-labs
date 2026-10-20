using CampusCafe.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCafe.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IDbContextFactory<CafeDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        if (await db.Drinks.AnyAsync()) return;

        db.Drinks.AddRange(
            new Drink { Name = "Капучино", Description = "Еспресо, молоко та ніжна пінка.", Icon = "☕", BasePrice = 65m },
            new Drink { Name = "Лате", Description = "М’який смак і більше молока.", Icon = "🥛", BasePrice = 70m },
            new Drink { Name = "Мокачино", Description = "Кава з шоколадом для солодкої паузи.", Icon = "🍫", BasePrice = 85m },
            new Drink { Name = "Матча-лате", Description = "Зелений чай матча з молоком.", Icon = "🍵", BasePrice = 90m },
            new Drink { Name = "Какао", Description = "Теплий шоколадний напій без кави.", Icon = "🤎", BasePrice = 60m },
            new Drink { Name = "Чай-лате", Description = "Пряний чай із молоком і корицею.", Icon = "🫖", BasePrice = 75m }
        );
        await db.SaveChangesAsync();
    }
}
