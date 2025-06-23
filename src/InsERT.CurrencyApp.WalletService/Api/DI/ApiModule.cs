using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace InsERT.CurrencyApp.WalletService.Api.DI;

public static class ApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ApiModule).Assembly);

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Wallet Service API",
                Version = "v1",
                Description = "API for managing user wallets and their balances",
                Contact = new OpenApiContact
                {
                    Name = "Pawel Szynal",
                    Email = "pawel@szynal.info"
                }
            });
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
