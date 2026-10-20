using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;
using RazorPagesCar.Models;
using RazorPagesCar.Services;

var builder = WebApplication.CreateBuilder(args);

// Контекст для автомобілів, відгуків та звернень
builder.Services.AddDbContext<RazorPagesCarContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RazorPagesCarContext") 
        ?? throw new InvalidOperationException("Connection string 'RazorPagesCarContext' not found.")));

// Контекст для Identity (користувачі, ролі)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RazorPagesCarContext") 
        ?? throw new InvalidOperationException("Connection string 'RazorPagesCarContext' not found.")));

// Налаштування Identity з ролями та вбудованим UI
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Політика "тільки Admin"
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
});

// Налаштування поштового сервісу
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();

// Закриваємо всю папку /Admin політикою адміністратора
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdministratorRole");
});

var app = builder.Build();

// Наповнення бази даних початковими даними та сидування адміністратора
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
    await IdentitySeeder.SeedAdminAsync(services, app.Configuration);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
