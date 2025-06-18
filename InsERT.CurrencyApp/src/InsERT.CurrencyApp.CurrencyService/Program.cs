using System.Reflection;

using InsERT.CurrencyApp.CurrencyService.Application.DI;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.Configuration.DI;
using InsERT.CurrencyApp.CurrencyService.Infrastructure;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Extensions;


try
{
    var builder = WebApplication.CreateBuilder(args);

    var appSettings = new AppSettings(builder.Configuration);
    builder.Services.AddSingleton(appSettings);

    IReadOnlyCollection<Assembly> serviceLayerAssemblies =
    [
        typeof(CurrencyRateFetcher).Assembly
    ];

    builder.Services
        .AddInfrastructure(appSettings)
        .AddApiServices(appSettings)
        .AddApplicationServices(appSettings, serviceLayerAssemblies)
        .AddJobScheduling(appSettings);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Middleware HTTP
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.UseCors();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Startup error: {ex.Message}");
    Environment.Exit(1);
}
finally
{
    Environment.Exit(0);
}
