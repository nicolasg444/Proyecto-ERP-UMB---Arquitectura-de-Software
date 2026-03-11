// ============================================================
// SaboresExoticos.API / Controllers / OrdersController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaboresExoticos.API.Data;
using SaboresExoticos.API.DTOs;
using SaboresExoticos.API.Models;

namespace SaboresExoticos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db) => _db = db;

    // ────────────────────────────────────────────────────────
    // GET api/orders  →  Lista de pedidos (cocina / ERP)
    // Filtros opcionales: ?status=Pending&date=2025-01-01
    // ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] DateTime? date)
    {
        var query = _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) &&
            Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            query = query.Where(o => o.Status == parsedStatus);

        if (date.HasValue)
            query = query.Where(o => o.CreatedAt.Date == date.Value.Date);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => ToDto(o))
            .ToListAsync();

        return Ok(new ApiResponse<List<OrderDto>>(true, "OK", orders));
    }

    // ────────────────────────────────────────────────────────
    // GET api/orders/{id}
    // ────────────────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound(new ApiResponse<OrderDto>(false, "Pedido no encontrado", null));

        return Ok(new ApiResponse<OrderDto>(true, "OK", ToDto(order)));
    }

    // ────────────────────────────────────────────────────────
    // POST api/orders  →  Crear pedido (cliente)
    // ────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Create([FromBody] CreateOrderRequest req)
    {
        // Validar que llegaron items
        if (req.Items == null || req.Items.Count == 0)
            return BadRequest(new ApiResponse<OrderDto>(false, "El pedido debe tener al menos un producto", null));

        // Obtener productos solicitados
        var productIds = req.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsAvailable)
            .ToListAsync();

        if (products.Count != productIds.Distinct().Count())
            return BadRequest(new ApiResponse<OrderDto>(false, "Uno o más productos no están disponibles", null));

        // Verificar stock
        foreach (var itemReq in req.Items)
        {
            var prod = products.First(p => p.Id == itemReq.ProductId);
            if (prod.Stock < itemReq.Quantity)
                return BadRequest(new ApiResponse<OrderDto>(
                    false, $"Stock insuficiente para '{prod.Name}' (disponible: {prod.Stock})", null));
        }

        // Asignar turno del día
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var counter = await _db.TurnCounters.FirstOrDefaultAsync(t => t.TurnDate == today);
        if (counter == null)
        {
            counter = new TurnCounter { TurnDate = today, LastTurn = 0 };
            _db.TurnCounters.Add(counter);
        }
        counter.LastTurn++;

        // Calcular total y tiempo estimado
        decimal total = 0;
        int maxPrepTime = 0;
        var items = new List<OrderItem>();

        foreach (var itemReq in req.Items)
        {
            var prod = products.First(p => p.Id == itemReq.ProductId);
            var unitPrice = prod.Price;
            total += unitPrice * itemReq.Quantity;
            maxPrepTime = Math.Max(maxPrepTime, prod.PrepTimeMin);

            items.Add(new OrderItem
            {
                ProductId = prod.Id,
                Quantity  = itemReq.Quantity,
                UnitPrice = unitPrice
            });

            // Descontar stock
            prod.Stock -= itemReq.Quantity;
            prod.UpdatedAt = DateTime.UtcNow;
        }

        // Pedidos pendientes activos afectan el tiempo estimado
        int pendingOrders = await _db.Orders
            .CountAsync(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress);
        int estimatedMinutes = maxPrepTime + (pendingOrders * 2);

        var order = new Order
        {
            TurnNumber       = counter.LastTurn,
            CustomerName     = req.CustomerName.Trim(),
            CustomerEmail    = req.CustomerEmail?.Trim(),
            Status           = OrderStatus.Pending,
            TotalAmount      = total,
            EstimatedMinutes = estimatedMinutes,
            Notes            = req.Notes?.Trim(),
            Items            = items
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Recargar con relaciones para la respuesta
        var created = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstAsync(o => o.Id == order.Id);

        return CreatedAtAction(nameof(GetById), new { id = order.Id },
            new ApiResponse<OrderDto>(true, "Pedido creado exitosamente", ToDto(created)));
    }

    // ────────────────────────────────────────────────────────
    // PATCH api/orders/{id}/status  →  Cambiar estado (cocina)
    // ────────────────────────────────────────────────────────
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateStatus(
        int id, [FromBody] UpdateOrderStatusRequest req)
    {
        var order = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound(new ApiResponse<OrderDto>(false, "Pedido no encontrado", null));

        if (!Enum.TryParse<OrderStatus>(req.Status, true, out var newStatus))
            return BadRequest(new ApiResponse<OrderDto>(false, $"Estado inválido: {req.Status}", null));

        // Reglas de transición
        var allowed = order.Status switch
        {
            OrderStatus.Pending    => new[] { OrderStatus.InProgress, OrderStatus.Cancelled },
            OrderStatus.InProgress => new[] { OrderStatus.Ready, OrderStatus.Cancelled },
            OrderStatus.Ready      => new[] { OrderStatus.Delivered },
            _                      => Array.Empty<OrderStatus>()
        };

        if (!allowed.Contains(newStatus))
            return BadRequest(new ApiResponse<OrderDto>(
                false, $"No se puede pasar de '{order.Status}' a '{newStatus}'", null));

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        if (newStatus == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<OrderDto>(true, $"Estado actualizado a {newStatus}", ToDto(order)));
    }

    // ────────────────────────────────────────────────────────
    // Mapeo privado
    // ────────────────────────────────────────────────────────
    private static OrderDto ToDto(Order o) => new(
        o.Id,
        o.TurnNumber,
        o.CustomerName,
        o.CustomerEmail,
        o.Status.ToString(),
        o.TotalAmount,
        o.EstimatedMinutes,
        o.Notes,
        o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(
            i.ProductId,
            i.Product?.Name ?? "Desconocido",
            i.Quantity,
            i.UnitPrice,
            i.Quantity * i.UnitPrice
        )).ToList()
    );
}
