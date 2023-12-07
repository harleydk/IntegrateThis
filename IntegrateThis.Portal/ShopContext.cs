using IntegrateThis.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrateThis.DAL;

public class ShopContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    public ShopContext(DbContextOptions<ShopContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Customer entity
        _ = modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
}