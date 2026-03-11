// ============================================================
// SaboresExoticos.API / Models / Customer.cs
// ============================================================
namespace SaboresExoticos.API.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// ============================================================
// SaboresExoticos.API / Models / Supplier.cs
// ============================================================
public class Supplier
{
    public int Id { get; set; }
    public string CommunityName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Certification { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================
// SaboresExoticos.API / Models / TurnCounter.cs
// ============================================================
public class TurnCounter
{
    public int Id { get; set; }
    public DateOnly TurnDate { get; set; }
    public int LastTurn { get; set; } = 0;
}
