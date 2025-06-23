using FluentValidation;

namespace InsERT.CurrencyApp.WalletService.Application.Behaviors;

public interface IValidationBehavior
{
    Task ValidateAsync<T>(T request, CancellationToken cancellationToken = default);
}

public sealed class ValidationBehavior(IServiceProvider serviceProvider) : IValidationBehavior
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(T));

        if (_serviceProvider.GetService(validatorType) is not IValidator<T> validator)
            return;

        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
