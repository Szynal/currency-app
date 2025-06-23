using FluentValidation;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;

namespace InsERT.CurrencyApp.TransactionService.Application.Validators;

public class ApplyTransactionCommandValidator : AbstractValidator<ApplyTransactionCommand>
{
    public ApplyTransactionCommandValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty()
            .WithMessage("WalletId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("CurrencyCode is required.")
            .Length(3)
            .WithMessage("CurrencyCode must be exactly 3 characters.")
            .Matches("^[A-Z]{3}$")
            .WithMessage("CurrencyCode must consist of uppercase letters.");

        When(x => x.Type == TransactionType.ConvertCurrency, () =>
        {
            RuleFor(x => x.ConvertedAmount)
                .NotNull()
                .WithMessage("ConvertedAmount is required for currency conversion.")
                .GreaterThan(0)
                .WithMessage("ConvertedAmount must be greater than zero.");

            RuleFor(x => x.ConvertedCurrencyCode)
                .NotEmpty()
                .WithMessage("ConvertedCurrencyCode is required for currency conversion.")
                .Length(3)
                .WithMessage("ConvertedCurrencyCode must be exactly 3 characters.")
                .Matches("^[A-Z]{3}$")
                .WithMessage("ConvertedCurrencyCode must consist of uppercase letters.");
        });
    }
}
