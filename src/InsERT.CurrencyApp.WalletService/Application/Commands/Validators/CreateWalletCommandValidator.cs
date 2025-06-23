using FluentValidation;

namespace InsERT.CurrencyApp.WalletService.Application.Commands.Validators;

public sealed class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Wallet name is required.")
            .MinimumLength(3)
            .WithMessage("Wallet name must be at least 3 characters long.")
            .MaximumLength(100)
            .WithMessage("Wallet name must be at most 100 characters long.")
            .Must(name => name!.Trim().Length > 0)
            .WithMessage("Wallet name cannot be whitespace.");
    }
}
