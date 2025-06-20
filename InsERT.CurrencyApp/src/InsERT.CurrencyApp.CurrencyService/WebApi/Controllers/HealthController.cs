using InsERT.CurrencyApp.Abstractions.Health;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;

[ApiController]
[Route("health")]
public class HealthController(IAppHealthService healthService) : ControllerBase
{
    private readonly IAppHealthService _healthService = healthService;

    [HttpGet("status")]
    [ProducesResponseType(typeof(AppHealthStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AppHealthStatus), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        var result = await _healthService.CheckHealthAsync();

        return result.Status.Equals("Healthy", StringComparison.OrdinalIgnoreCase)
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
