using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<RazorPagesCarContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RazorPagesCarContext") 
        ?? throw new InvalidOperationException("Connection string 'RazorPagesCarContext' not found.")));

var app = builder.Build();

// Наповнення бази початковими даними (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
