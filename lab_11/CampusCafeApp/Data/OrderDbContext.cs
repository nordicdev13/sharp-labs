using CampusCafe.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCafe.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<PurchaseOrder> Orders => Set<PurchaseOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>().HasIndex(o => o.RequestId).IsUnique();
        modelBuilder.Entity<PurchaseOrder>().HasMany(o => o.Items).WithOne()
            .HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
