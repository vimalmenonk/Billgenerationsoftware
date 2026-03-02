using GlowvitraBilling.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlowvitraBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly BillingDbContext _dbContext;

    public ProductsController(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var normalized = query.Trim().ToLower();
        var products = await _dbContext.Products
            .Where(p => p.Name.ToLower().Contains(normalized))
            .OrderBy(p => p.Name)
            .Take(10)
            .ToListAsync();

        return Ok(products);
    }
}
