using FluentValidation;

namespace InsERT.CurrencyApp.WalletService.Application.Behaviors;

public interface IValidationBehavior
{
    Task ValidateAsync<T>(T request, CancellationToken cancellationToken = default);
}

public sealed class ValidationBehavior : IValidationBehavior
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
