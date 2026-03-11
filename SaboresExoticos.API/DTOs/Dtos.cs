// ============================================================
// SaboresExoticos.API / DTOs / ProductDtos.cs
// ============================================================
namespace SaboresExoticos.API.DTOs;

// ── Products ────────────────────────────────────────────────
public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Region,
    int PrepTimeMin,
    string? ImageUrl,
    bool IsAvailable,
    int Stock,
    bool LowStock        // Stock <= MinStock
);

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Region,
    int PrepTimeMin,
    string? ImageUrl,
    int Stock,
    int MinStock
);

public record UpdateStockRequest(int NewStock);

// ── Orders ──────────────────────────────────────────────────
public record CreateOrderRequest(
    string CustomerName,
    string? CustomerEmail,
    string? Notes,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(int ProductId, int Quantity);

public record OrderDto(
    int Id,
    int TurnNumber,
    string CustomerName,
    string? CustomerEmail,
    string Status,
    decimal TotalAmount,
    int EstimatedMinutes,
    string? Notes,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record UpdateOrderStatusRequest(string Status);

// ── Auth ────────────────────────────────────────────────────
public record RegisterRequest(string Name, string Email, string? Phone, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Name, string Email);

// ── Suppliers ───────────────────────────────────────────────
public record SupplierDto(
    int Id,
    string CommunityName,
    string Region,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Certification,
    bool IsActive
);

public record CreateSupplierRequest(
    string CommunityName,
    string Region,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Certification
);

// ── Dashboard ───────────────────────────────────────────────
public record DashboardDto(
    decimal TodaySales,
    int TodayOrders,
    string TopProduct,
    double AvgAttentionMinutes,
    List<ProductSalesDto> SalesByProduct
);

public record ProductSalesDto(string ProductName, int Quantity, decimal Revenue);

// ── Generic ─────────────────────────────────────────────────
public record ApiResponse<T>(bool Success, string Message, T? Data);
