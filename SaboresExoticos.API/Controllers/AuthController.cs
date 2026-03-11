// ============================================================
// SaboresExoticos.API / Controllers / AuthController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SaboresExoticos.API.Data;
using SaboresExoticos.API.DTOs;
using SaboresExoticos.API.Models;

namespace SaboresExoticos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest req)
    {
        if (await _db.Customers.AnyAsync(c => c.Email == req.Email.ToLower()))
            return Conflict(new ApiResponse<AuthResponse>(false, "El email ya está registrado", null));

        var customer = new Customer
        {
            Name         = req.Name.Trim(),
            Email        = req.Email.Trim().ToLower(),
            Phone        = req.Phone?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var token = GenerateToken(customer);
        return Ok(new ApiResponse<AuthResponse>(true, "Registro exitoso",
            new AuthResponse(token, customer.Name, customer.Email)));
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest req)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.Email == req.Email.ToLower());

        if (customer == null || !BCrypt.Net.BCrypt.Verify(req.Password, customer.PasswordHash))
            return Unauthorized(new ApiResponse<AuthResponse>(false, "Credenciales inválidas", null));

        var token = GenerateToken(customer);
        return Ok(new ApiResponse<AuthResponse>(true, "Login exitoso",
            new AuthResponse(token, customer.Name, customer.Email)));
    }

    private string GenerateToken(Customer customer)
    {
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Name, customer.Name)
        };

        var token = new JwtSecurityToken(
            issuer:   _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims:   claims,
            expires:  DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// ============================================================
// SaboresExoticos.API / Controllers / SuppliersController.cs
// ============================================================
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _db;
    public SuppliersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SupplierDto>>>> GetAll()
    {
        var list = await _db.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Region)
            .Select(s => ToDto(s))
            .ToListAsync();

        return Ok(new ApiResponse<List<SupplierDto>>(true, "OK", list));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SupplierDto>>> Create([FromBody] CreateSupplierRequest req)
    {
        var supplier = new Supplier
        {
            CommunityName = req.CommunityName.Trim(),
            Region        = req.Region.Trim(),
            ContactName   = req.ContactName?.Trim(),
            Phone         = req.Phone?.Trim(),
            Email         = req.Email?.Trim(),
            Certification = req.Certification?.Trim()
        };
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<SupplierDto>(true, "Proveedor creado", ToDto(supplier)));
    }

    private static SupplierDto ToDto(Supplier s) => new(
        s.Id, s.CommunityName, s.Region, s.ContactName,
        s.Phone, s.Email, s.Certification, s.IsActive);
}

// ============================================================
// SaboresExoticos.API / Controllers / DashboardController.cs
// ============================================================
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    // GET api/dashboard
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> Get()
    {
        var today = DateTime.UtcNow.Date;

        var todayOrders = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.CreatedAt.Date == today && o.Status != OrderStatus.Cancelled)
            .ToListAsync();

        var todaySales   = todayOrders.Sum(o => o.TotalAmount);
        var todayCount   = todayOrders.Count;

        // Tiempo promedio (solo entregados)
        var delivered = todayOrders.Where(o => o.DeliveredAt.HasValue).ToList();
        var avgMinutes = delivered.Count > 0
            ? delivered.Average(o => (o.DeliveredAt!.Value - o.CreatedAt).TotalMinutes)
            : 0;

        // Ventas por producto
        var salesByProduct = todayOrders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Product?.Name ?? "Desconocido")
            .Select(g => new ProductSalesDto(
                g.Key,
                g.Sum(i => i.Quantity),
                g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(x => x.Quantity)
            .ToList();

        var topProduct = salesByProduct.FirstOrDefault()?.ProductName ?? "Sin ventas";

        return Ok(new ApiResponse<DashboardDto>(true, "OK", new DashboardDto(
            todaySales, todayCount, topProduct,
            Math.Round(avgMinutes, 1), salesByProduct)));
    }
}
