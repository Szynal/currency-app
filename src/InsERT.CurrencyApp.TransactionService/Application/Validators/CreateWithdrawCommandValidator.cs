using FluentValidation;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Validators;

public class CreateWithdrawCommandValidator : AbstractValidator<CreateWithdrawCommand>
{
    public CreateWithdrawCommandValidator(ICurrencyServiceClient currencyServiceClient)
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .MustAsync(async (code, ct) =>
            {
                var availableCodes = await currencyServiceClient.GetAvailableCurrencyCodesAsync(ct);
                return availableCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
            })
            .WithMessage("Currency code is not supported.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
