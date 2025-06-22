using InsERT.CurrencyApp.Abstractions.Health;
using InsERT.CurrencyApp.CurrencyService.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InsERT.CurrencyApp.CurrencyService.Tests.WebApi.Controllers;

public class HealthControllerTests
{
    [Fact]
    public async Task GetHealth_WhenHealthy_ReturnsOk()
    {
        // Arrange
        var mockHealthService = new Mock<IAppHealthService>();
        mockHealthService
            .Setup(s => s.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppHealthStatus
            {
                Status = "Healthy",
                Duration = TimeSpan.FromSeconds(1),
                Checks = []
            });

        var controller = new HealthController(mockHealthService.Object);

        // Act
        var result = await controller.GetHealth();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var status = Assert.IsType<AppHealthStatus>(okResult.Value);
        Assert.Equal("Healthy", status.Status);
    }

    [Fact]
    public async Task GetHealth_WhenUnhealthy_ReturnsServiceUnavailable()
    {
        // Arrange
        var mockHealthService = new Mock<IAppHealthService>();
        mockHealthService
            .Setup(s => s.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppHealthStatus
            {
                Status = "Unhealthy",
                Duration = TimeSpan.FromSeconds(1),
                Checks = []
            });

        var controller = new HealthController(mockHealthService.Object);

        // Act
        var result = await controller.GetHealth();

        // Assert
        var serviceUnavailableResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, serviceUnavailableResult.StatusCode);
        var status = Assert.IsType<AppHealthStatus>(serviceUnavailableResult.Value);
        Assert.Equal("Unhealthy", status.Status);
    }
}
