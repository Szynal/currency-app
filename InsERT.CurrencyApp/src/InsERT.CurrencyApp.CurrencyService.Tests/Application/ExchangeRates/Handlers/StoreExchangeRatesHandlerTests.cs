using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;
using InsERT.CurrencyApp.CurrencyService.DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace InsERT.CurrencyApp.CurrencyService.Tests.Application.ExchangeRates.Handlers;

public class StoreExchangeRatesHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private CurrencyDbContext _dbContext = null!;

    public StoreExchangeRatesHandlerTests()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("currencydb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:15-alpine")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<CurrencyDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString()) 
            .Options;

        _dbContext = new CurrencyDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task Should_Store_New_ExchangeRates()
    {
        // Arrange
        var handler = new StoreExchangeRatesHandler(_dbContext);
        var command = new StoreExchangeRatesCommand(new()
        {
            EffectiveDate = DateTime.Today.ToString("yyyy-MM-dd"),
            Table = "B",
            Rates =
            [
                new() { Code = "USD", Currency = "US Dollar", Mid = 4.25m },
                new() { Code = "EUR", Currency = "Euro", Mid = 4.60m }
            ]
        });

        // Act
        var insertedCount = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(2, insertedCount);

        var storedRates = await _dbContext.ExchangeRates.ToListAsync();
        Assert.Equal(2, storedRates.Count);
        Assert.Contains(storedRates, r => r.Code == "USD" && r.Rate == 4.25m);
        Assert.Contains(storedRates, r => r.Code == "EUR" && r.Rate == 4.60m);
    }
}
