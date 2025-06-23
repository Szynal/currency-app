using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InsERT.CurrencyApp.CurrencyService.Tests.WebApi.Controllers;

public class NbpTableBRatesControllerTests
{
    private readonly Mock<IQueryDispatcher> _dispatcherMock;
    private readonly NbpTableBRatesController _controller;

    public NbpTableBRatesControllerTests()
    {
        _dispatcherMock = new Mock<IQueryDispatcher>();
        _controller = new NbpTableBRatesController(_dispatcherMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenRatesExist()
    {
        // Arrange
        var expected = new List<ExchangeRateDto>
        {
            new() { Code = "XPF", Currency = "frank CFA BCEAO ", Rate = 0.006538m, EffectiveDate = DateOnly.FromDateTime(DateTime.Today) }
        };

        _dispatcherMock
            .Setup(d => d.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
                It.IsAny<GetExchangeRatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAll(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var rates = Assert.IsAssignableFrom<IEnumerable<ExchangeRateDto>>(okResult.Value);
        Assert.Single(rates);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNoContent_WhenNoRates()
    {
        // Arrange
        _dispatcherMock
            .Setup(d => d.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
                It.IsAny<GetExchangeRatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetAll(null, null);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldPassCorrectParameters()
    {
        // Arrange
        var date = new DateOnly(2024, 01, 01);
        var code = "EUR";

        _dispatcherMock
            .Setup(d => d.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
                It.Is<GetExchangeRatesQuery>(q => q.Date == date && q.Code == code),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _controller.GetAll(date, code);

        // Assert
        _dispatcherMock.VerifyAll();
    }
}
