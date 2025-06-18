using System.Text.Json;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

public interface INbpCurrencyClient
{
    Task<NbpTable?> GetLatestTableAsync(CancellationToken ct);
}

public class NbpCurrencyClient : INbpCurrencyClient
{
    private const string Url = "https://api.nbp.pl/api/exchangerates/tables/B/?format=json";
    private readonly IHttpClientFactory _http;

    public NbpCurrencyClient(IHttpClientFactory http)
    {
        _http = http;
    }

    public async Task<NbpTable?> GetLatestTableAsync(CancellationToken ct)
    {
        var client = _http.CreateClient();
        var response = await client.GetAsync(Url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var data = JsonSerializer.Deserialize<List<NbpTable>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data?.FirstOrDefault();
    }
}
