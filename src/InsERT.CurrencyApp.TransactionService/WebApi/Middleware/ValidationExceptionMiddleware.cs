using FluentValidation;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Exceptions;
using System.Text.Json;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Middleware;

public class ValidationExceptionMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new
            {
                title = "Validation Failed",
                status = StatusCodes.Status422UnprocessableEntity,
                errors = ex.Errors.ToDictionary(
                    e => e.PropertyName,
                    e => new[] { e.ErrorMessage }
                )
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, _jsonOptions));
        }
        catch (WalletServiceException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            context.Response.ContentType = "application/problem+json";

            string? error = null;
            string? exceptionMessage = null;

            try
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(ex.Message);

                parsed?.TryGetValue("error", out error);
                parsed?.TryGetValue("exceptionMessage", out exceptionMessage);
            }
            catch
            {
                exceptionMessage = ex.Message;
            }

            var problemDetails = new
            {
                title = error ?? "Wallet Service Error",
                status = (int)ex.StatusCode,
                detail = exceptionMessage ?? "An unexpected error occurred in Wallet Service"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, _jsonOptions));
        }
    }
}
