using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Models;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Queries;
using InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InsERT.CurrencyApp.CurrencyService.Tests.WebApi.Controllers;

public class RatesControllerTests
{
    private readonly Mock<IQueryDispatcher> _dispatcherMock;
    private readonly RatesController _controller;

    public RatesControllerTests()
    {
        _dispatcherMock = new Mock<IQueryDispatcher>();
        _controller = new RatesController(_dispatcherMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenRatesExist()
    {
        // Arrange
        var expected = new List<ExchangeRateDto>
        {
            new() { Code = "USD", Currency = "US Dollar", Rate = 4.20m, EffectiveDate = DateOnly.FromDateTime(DateTime.Today) }
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
        Assert.IsType<NoContentResult>(result);
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
