using FluentValidation;

namespace InsERT.CurrencyApp.WalletService.Application.Commands.Validators;

public sealed class AddWalletBalanceCommandValidator : AbstractValidator<AddWalletBalanceCommand>
{
    public AddWalletBalanceCommandValidator()
    {
        RuleFor(x => x.WalletId).NotEmpty();
        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .Length(3).WithMessage("CurrencyCode must be exactly 3 letters.");
        RuleFor(x => x.InitialAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Initial amount must be non-negative.");
    }
}
