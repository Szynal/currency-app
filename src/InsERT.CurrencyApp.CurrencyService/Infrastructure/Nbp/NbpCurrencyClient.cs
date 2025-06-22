using System.Text.Json;
using InsERT.CurrencyApp.Abstractions.Serialization;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

public class NbpCurrencyClient(
    HttpClient httpClient,
    ILogger<NbpCurrencyClient> logger,
    IOptions<NbpClientSettings> options) : INbpCurrencyClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<NbpCurrencyClient> _logger = logger;
    private readonly Uri _endpoint = NbpUriFactory.BuildTableEndpoint(options.Value);
    private static readonly JsonSerializerOptions JsonOptions = JsonSerializationProfiles.CaseInsensitive;

    public async Task<NbpTable?> FetchLatestRatesTableAsync(CancellationToken ct)
    {
        _logger.LogInformation("Calling NBP API: {Url}", _endpoint);

        try
        {
            var response = await _httpClient.GetAsync(_endpoint, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var data = JsonSerializer.Deserialize<List<NbpTable>>(json, JsonOptions);

            return data?.FirstOrDefault();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to NBP API failed.");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from NBP API.");
            throw;
        }
    }
}