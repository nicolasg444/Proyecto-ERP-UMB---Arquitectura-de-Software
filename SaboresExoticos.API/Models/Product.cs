// ============================================================
// SaboresExoticos.API / Models / Product.cs
// ============================================================
namespace SaboresExoticos.API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Region { get; set; } = string.Empty;
    public int PrepTimeMin { get; set; } = 5;
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int Stock { get; set; } = 50;
    public int MinStock { get; set; } = 10;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
