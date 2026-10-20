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
}
