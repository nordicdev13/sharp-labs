using CampusCafe.Components;
using CampusCafe.Data;
using CampusCafe.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var databasePath = Path.Combine(builder.Environment.ContentRootPath, "cafe.db");
builder.Services.AddDbContextFactory<CafeDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<OrderState>();
// Каталог попередньої роботи не змінюємо. Замовлення мають окрему навчальну БД.
var ordersPath = Path.Combine(builder.Environment.ContentRootPath, "orders.db");
builder.Services.AddDbContextFactory<OrderDbContext>(options =>
    options.UseSqlite($"Data Source={ordersPath}"));
builder.Services.AddScoped<PurchaseService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/api/drinks", async (CatalogService catalog) =>
    await catalog.GetDrinksAsync());
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await SeedData.InitializeAsync(
    app.Services.GetRequiredService<IDbContextFactory<CafeDbContext>>());
await using (var ordersDb = await app.Services
    .GetRequiredService<IDbContextFactory<OrderDbContext>>().CreateDbContextAsync())
{
    await ordersDb.Database.EnsureCreatedAsync();
}

app.Run();
