using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;

[ApiController]
[Route("nbp/table-b/codes")]
public class CurrencyCodesController(IQueryDispatcher dispatcher) : ControllerBase
{
    private readonly IQueryDispatcher _dispatcher = dispatcher;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await _dispatcher.QueryAsync<GetCurrencyCodesQuery, IEnumerable<string>>(new());
        return Ok(result);
    }
}
