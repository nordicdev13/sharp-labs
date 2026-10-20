using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Data;

namespace RazorPagesCar.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new RazorPagesCarContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<RazorPagesCarContext>>()))
        {
            if (context == null || context.Car == null)
            {
                throw new ArgumentNullException("Null RazorPagesCarContext");
            }

            // Перевіряємо, чи в базі вже є записи
            if (context.Car.Any())
            {
                return; // База даних уже наповнена
            }

            context.Car.AddRange(
                new Car
                {
                    Make = "Audi",
                    Model = "RS6 Avant",
                    Year = 2023,
                    Price = 125000M,
                    BodyType = "Універсал",
                    Color = "Сірий"
                },
                new Car
                {
                    Make = "Porsche",
                    Model = "911 Carrera",
                    Year = 2024,
                    Price = 140000M,
                    BodyType = "Купе",
                    Color = "Червоний"
                },
                new Car
                {
                    Make = "Dodge",
                    Model = "Challenger SRT",
                    Year = 2022,
                    Price = 68000M,
                    BodyType = "Купе",
                    Color = "Чорний"
                },
                new Car
                {
                    Make = "BMW",
                    Model = "M5 Competition",
                    Year = 2023,
                    Price = 112000M,
                    BodyType = "Седан",
                    Color = "Синій"
                },
                new Car
                {
                    Make = "Audi",
                    Model = "Q7",
                    Year = 2024,
                    Price = 85000M,
                    BodyType = "Позашляховик",
                    Color = "Білий"
                },
                new Car
                {
                    Make = "BMW",
                    Model = "X5 M",
                    Year = 2023,
                    Price = 118000M,
                    BodyType = "Позашляховик",
                    Color = "Чорний"
                }
            );

            context.SaveChanges();
        }
    }
}
