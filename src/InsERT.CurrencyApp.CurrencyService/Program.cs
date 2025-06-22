using InsERT.CurrencyApp.CurrencyService.Application.DI;
using InsERT.CurrencyApp.CurrencyService.Configuration.DI;
using InsERT.CurrencyApp.CurrencyService.Infrastructure;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfiguration(builder.Configuration)
    .AddInfrastructure()
    .AddApplication()
    .AddApi()
    .AddHostedJobs();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    await DbInitializer.EnsureDatabaseMigratedAsync(app.Services, logger);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();
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
