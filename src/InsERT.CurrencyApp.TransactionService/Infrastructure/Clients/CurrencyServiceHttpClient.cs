using InsERT.CurrencyApp.Abstractions.Currency.Models;
using System.Text.Json;
using System.Web;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

public class CurrencyServiceHttpClient(HttpClient httpClient, ILogger<CurrencyServiceHttpClient> logger) : ICurrencyServiceClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<CurrencyServiceHttpClient> _logger = logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(DateOnly? date, string? code, CancellationToken cancellationToken)
    {
        var url = "nbp/table-b/rates";
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (date.HasValue) query["date"] = date.Value.ToString("yyyy-MM-dd");
        if (!string.IsNullOrEmpty(code)) query["code"] = code;
        if (query.Count > 0) url += "?" + query;

        try
        {
            var rates = await _httpClient.GetFromJsonAsync<IEnumerable<ExchangeRateDto>>(url, _jsonOptions, cancellationToken);
            return rates ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching exchange rates");
            return [];
        }
    }
}
