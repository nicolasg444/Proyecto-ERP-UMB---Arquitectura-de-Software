// ============================================================
// SaboresExoticos.API / Data / AppDbContext.cs
// ============================================================
using Microsoft.EntityFrameworkCore;
using SaboresExoticos.API.Models;

namespace SaboresExoticos.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<TurnCounter> TurnCounters => Set<TurnCounter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Region).HasMaxLength(100).IsRequired();
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");
            e.Property(o => o.Status)
             .HasConversion<string>()
             .HasMaxLength(20);
            e.Property(o => o.CustomerName).HasMaxLength(150).IsRequired();
        });

        // OrderItem - sin columna calculada en EF, se calcula en memoria
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");
            e.Ignore(i => i.Subtotal); // Calculado en C#, no en BD
        });

        // Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasIndex(c => c.Email).IsUnique();
            e.Property(c => c.Email).HasMaxLength(200).IsRequired();
        });

        // TurnCounter
        modelBuilder.Entity<TurnCounter>(e =>
        {
            e.HasIndex(t => t.TurnDate).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
