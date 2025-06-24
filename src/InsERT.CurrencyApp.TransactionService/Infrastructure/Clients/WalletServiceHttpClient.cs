using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Exceptions;
using System.Net;
using System.Text;
using System.Text.Json;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

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

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            string? errorMessage = null;

            try
            {
                using var doc = JsonDocument.Parse(errorContent);
                if (doc.RootElement.TryGetProperty("exceptionMessage", out var messageElement))
                {
                    errorMessage = messageElement.GetString();
                }
                else if (doc.RootElement.TryGetProperty("error", out var errorElement))
                {
                    errorMessage = errorElement.GetString();
                }
            }
            catch
            {
                errorMessage = null;
            }

            throw new WalletServiceException(
                response.StatusCode,
                errorMessage ?? $"WalletService returned {(int)response.StatusCode} {response.StatusCode}"
            );
        }

    }
}