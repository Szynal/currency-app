using FluentValidation;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Validators;

public class CreateConversionCommandValidator : AbstractValidator<CreateConversionCommand>
{
    public CreateConversionCommandValidator(ICurrencyServiceClient currencyServiceClient)
    {
        RuleFor(x => x.SourceCurrencyCode)
            .NotEmpty().WithMessage("Source currency code is required.")
            .MustAsync(async (code, ct) =>
            {
                var availableCodes = await currencyServiceClient.GetAvailableCurrencyCodesAsync(ct);
                return availableCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
            }).WithMessage("Source currency code is not supported.");

        RuleFor(x => x.TargetCurrencyCode)
            .NotEmpty().WithMessage("Target currency code is required.")
            .MustAsync(async (code, ct) =>
            {
                var availableCodes = await currencyServiceClient.GetAvailableCurrencyCodesAsync(ct);
                return availableCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
            }).WithMessage("Target currency code is not supported.")
            .NotEqual(x => x.SourceCurrencyCode).WithMessage("Target currency must differ from source.");

        RuleFor(x => x.SourceAmount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
