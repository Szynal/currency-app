using Microsoft.OpenApi.Models;

namespace InsERT.CurrencyApp.CurrencyService.Configuration.DI;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(
        this IServiceCollection services)
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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Currency Service API",
                Version = "v1"
            });
        });

        return services;
    }

}
