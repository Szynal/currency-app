using FluentValidation.TestHelper;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.Commands.Validators;

namespace InsERT.CurrencyApp.WalletService.Tests.Validators;

public class CreateWalletCommandValidatorTests
{
    private readonly CreateWalletCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.Empty,
            Name = "Travel"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Short()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.NewGuid(),
            Name = "a"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Long()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.NewGuid(),
            Name = new string('a', 101)
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Only_Whitespace()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.NewGuid(),
            Name = "   "
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.NewGuid(),
            Name = null!
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateWalletCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Savings"
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
