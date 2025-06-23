using InsERT.CurrencyApp.WalletService.Api.DI;
using InsERT.CurrencyApp.WalletService.Application.DI;
using InsERT.CurrencyApp.WalletService.Configuration.DI;
using InsERT.CurrencyApp.WalletService.Infrastructure;
using InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;
using InsERT.CurrencyApp.WalletService.Infrastructure.DI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfiguration(builder.Configuration)
    .AddInfrastructure()
    .AddApplication()
    .AddApi();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    await DbInitializer.EnsureDatabaseMigratedAsync<WalletDbContext>(app.Services, logger);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseAuthorization();
    app.UseCors();
    app.MapControllers();

    await app.RunAsync();
}
catch (OptionsValidationException ex)
{
    logger.LogCritical("Configuration validation failed:\n - {Failures}", string.Join("\n - ", ex.Failures));
    Environment.Exit(1);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Startup error");
    Environment.Exit(1);
}
