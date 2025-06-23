using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;

[ApiController]
[Route("nbp/table-b/rates")]
public class NbpTableBRatesController(IQueryDispatcher dispatcher) : ControllerBase
{
    private readonly IQueryDispatcher _dispatcher = dispatcher;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? code)
    {
        var query = new GetExchangeRatesQuery(date, code);
        var result = await _dispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(query);

        if (!result.Any())
        {
            return NotFound($"No exchange rates found for code '{code}' in table B.");
        }

        return Ok(result);
    }
}
