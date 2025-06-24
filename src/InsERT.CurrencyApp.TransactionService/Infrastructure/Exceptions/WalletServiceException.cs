using System.Net;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Exceptions;

public class WalletServiceException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
