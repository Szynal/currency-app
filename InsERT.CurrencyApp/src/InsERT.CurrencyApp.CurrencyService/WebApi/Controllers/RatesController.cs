using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Models;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;

[ApiController]
[Route("rates")]
public class RatesController(IQueryDispatcher dispatcher) : ControllerBase
{
    private readonly IQueryDispatcher _dispatcher = dispatcher;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? code)
    {
        var query = new GetExchangeRatesQuery(date, code);
        var result = await _dispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(query);

        if (!result.Any())
        {
            return NoContent();
        }

        return Ok(result);
    }
}
