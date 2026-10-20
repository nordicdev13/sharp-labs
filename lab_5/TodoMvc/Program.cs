using Microsoft.EntityFrameworkCore;
using TodoMvc.Data;
using TodoMvc.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// SQLite: appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=todo.db"));

var app = builder.Build();

// Авто-міграція + початкові дані (сідінг)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.TodoItems.Any())
    {
        db.TodoItems.AddRange(
            new TodoItem
            {
                Title = "Спробувати додаток",
                Description = "Ознайомитися з інтерфейсом та перевірити роботу",
                Priority = Priority.High,
                DueAt = DateTime.UtcNow.AddDays(1)
            },
            new TodoItem
            {
                Title = "Додати нове завдання",
                Description = "Створити завдання з власним дедлайном та описом",
                Priority = Priority.Medium
            },
            new TodoItem
            {
                Title = "Позначити виконаним",
                Description = "Перевірити зміну статусу натисканням однієї кнопки",
                Priority = Priority.Low,
                IsDone = true
            }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TodoItems}/{action=Index}/{id?}");

app.Run();
