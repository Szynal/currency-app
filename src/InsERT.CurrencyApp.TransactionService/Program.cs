using InsERT.CurrencyApp.TransactionService.Application.DI;
using InsERT.CurrencyApp.TransactionService.Configuration.DI;
using InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;
using InsERT.CurrencyApp.TransactionService.Infrastructure.DI;
using InsERT.CurrencyApp.TransactionService.Migrations;
using InsERT.CurrencyApp.TransactionService.WebApi.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfiguration(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddApi()
    .AddHostedJobs();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    await DbInitializer.EnsureDatabaseMigratedAsync<TransactionDbContext>(app.Services, logger);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseMiddleware<ValidationExceptionMiddleware>();
    app.UseAuthorization();
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
