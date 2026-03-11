// ============================================================
// SaboresExoticos.API / Models / Order.cs
// ============================================================
namespace SaboresExoticos.API.Models;

public enum OrderStatus
{
    Pending,
    InProgress,
    Ready,
    Delivered,
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public int TurnNumber { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public int EstimatedMinutes { get; set; } = 10;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }

    // FK opcional a cliente registrado
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Navegación
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

// ============================================================
// SaboresExoticos.API / Models / OrderItem.cs
// ============================================================
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}
