using GlowvitraBilling.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlowvitraBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly BillingDbContext _dbContext;

    public LookupController(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("seller")]
    public async Task<IActionResult> GetSeller()
    {
        var seller = await _dbContext.SellerProfiles
            .Include(s => s.State)
            .Select(s => new
            {
                s.Id,
                s.SellerName,
                s.Address,
                s.Gstin,
                s.StateId,
                StateName = s.State != null ? s.State.Name : string.Empty
            })
            .FirstAsync();

        return Ok(seller);
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetStates()
    {
        var states = await _dbContext.States.OrderBy(s => s.Name).ToListAsync();
        return Ok(states);
    }
}
