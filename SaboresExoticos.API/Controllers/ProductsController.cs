// ============================================================
// SaboresExoticos.API / Controllers / ProductsController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaboresExoticos.API.Data;
using SaboresExoticos.API.DTOs;
using SaboresExoticos.API.Models;

namespace SaboresExoticos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    // GET api/products  →  Catálogo completo (solo disponibles)
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAll([FromQuery] bool all = false)
    {
        var query = _db.Products.AsQueryable();
        if (!all) query = query.Where(p => p.IsAvailable);

        var products = await query
            .OrderBy(p => p.Name)
            .Select(p => ToDto(p))
            .ToListAsync();

        return Ok(new ApiResponse<List<ProductDto>>(true, "OK", products));
    }

    // GET api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound(new ApiResponse<ProductDto>(false, "Producto no encontrado", null));
        return Ok(new ApiResponse<ProductDto>(true, "OK", ToDto(p)));
    }

    // POST api/products  →  Crear producto (admin)
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new ApiResponse<ProductDto>(false, "El nombre es requerido", null));

        var product = new Product
        {
            Name        = req.Name.Trim(),
            Description = req.Description.Trim(),
            Price       = req.Price,
            Region      = req.Region.Trim(),
            PrepTimeMin = req.PrepTimeMin,
            ImageUrl    = req.ImageUrl,
            Stock       = req.Stock,
            MinStock    = req.MinStock
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new ApiResponse<ProductDto>(true, "Producto creado", ToDto(product)));
    }

    // PATCH api/products/{id}/stock  →  Actualizar stock
    [HttpPatch("{id:int}/stock")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateStock(int id, [FromBody] UpdateStockRequest req)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound(new ApiResponse<ProductDto>(false, "Producto no encontrado", null));

        p.Stock = req.NewStock;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<ProductDto>(true, "Stock actualizado", ToDto(p)));
    }

    // DELETE api/products/{id}  →  Deshabilitar (soft delete)
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Disable(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound(new ApiResponse<object>(false, "Producto no encontrado", null));

        p.IsAvailable = false;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>(true, "Producto deshabilitado", null));
    }

    // Mapeo privado
    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.Region,
        p.PrepTimeMin, p.ImageUrl, p.IsAvailable, p.Stock,
        p.Stock <= p.MinStock
    );
}
