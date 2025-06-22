using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace InsERT.CurrencyApp.WalletService.Application.Wallets.Models;

public partial class CreateWalletRequest : IValidatableObject
{
    [Required(ErrorMessage = "Wallet name is required.")]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult("Wallet name is required.", [nameof(Name)]);
        }

        if (!WalletNameValidationRegex().IsMatch(Name))
        {
            yield return new ValidationResult(
                "Wallet name contains unsupported characters.",
                [nameof(Name)]);
        }
    }

    [GeneratedRegex(@"^[\w\s\-.,()#@!$%&'*+/=?^_`{|}~]+$", RegexOptions.Compiled)]
    private static partial Regex WalletNameValidationRegex();
}
