using FluentValidation;
using FluentValidation.AspNetCore;
using InsERT.CurrencyApp.TransactionService.Application.Validators;
using Microsoft.OpenApi.Models;

namespace InsERT.CurrencyApp.TransactionService.Configuration.DI;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllers();
        services.AddValidatorsFromAssemblyContaining<CreateDepositCommandValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Transaction Service API",
                Version = "v1",
                Description = "API for processing transactions in CurrencyApp",
                Contact = new OpenApiContact
                {
                    Name = "Pawel",
                    Email = "pawel@szynal.info"
                }
            });
        });

        return services;
    }
}
