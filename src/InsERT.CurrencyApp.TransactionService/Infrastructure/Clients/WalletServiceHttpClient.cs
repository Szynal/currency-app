using System.Net.Http;
using System.Text;
using System.Text.Json;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

public interface IWalletServiceClient
{
    Task ApplyTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
}

public class WalletServiceHttpClient(HttpClient client) : IWalletServiceClient
{
    private readonly HttpClient _client = client;
    private const string ApplyTransactionEndpoint = "/api/wallet/apply-transaction";

    public async Task ApplyTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var payload = new
        {
            transaction.WalletId,
            TransactionId = transaction.Id,
            Type = transaction.Type.ToString(),
            transaction.Amount,
            transaction.CurrencyCode,
            transaction.ConvertedAmount,
            transaction.ConvertedCurrencyCode
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(ApplyTransactionEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
