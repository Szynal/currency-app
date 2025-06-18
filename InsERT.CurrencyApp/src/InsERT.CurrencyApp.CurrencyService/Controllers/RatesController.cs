using InsERT.CurrencyApp.CurrencyService.Domain;
using InsERT.CurrencyApp.CurrencyService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Controllers;

[ApiController]
[Route("rates")]
public class RatesController(CurrencyDbContext db) : ControllerBase
{
    private readonly CurrencyDbContext _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? code)
    {
        var query = _db.ExchangeRates.AsQueryable();

        if (date is not null)
        {
            query = query.Where(r => r.EffectiveDate == date.Value);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(r => r.Code.Equals(code, StringComparison.CurrentCultureIgnoreCase));
        }

        var results = await query
            .OrderByDescending(r => r.EffectiveDate)
            .ToListAsync();

        return Ok(results);
    }

}
