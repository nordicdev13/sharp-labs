using CampusCafe.Data;
using CampusCafe.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCafe.Services;

public class CatalogService
{
    private readonly IDbContextFactory<CafeDbContext> factory;

    public CatalogService(IDbContextFactory<CafeDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<List<Drink>> GetDrinksAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Drinks.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
    }
}
