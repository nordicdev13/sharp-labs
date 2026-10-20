using Microsoft.EntityFrameworkCore;
using RazorPagesCar.Models;

namespace RazorPagesCar.Data;

public class RazorPagesCarContext : DbContext
{
    public RazorPagesCarContext(DbContextOptions<RazorPagesCarContext> options)
        : base(options)
    {
    }

    public DbSet<Car> Car { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Один відгук від користувача на один автомобіль
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.CarId, r.UserId })
            .IsUnique();

        // Каскадне видалення відгуків при видаленні авто
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
