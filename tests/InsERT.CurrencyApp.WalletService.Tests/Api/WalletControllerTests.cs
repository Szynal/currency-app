using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Queries;
using InsERT.CurrencyApp.WalletService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InsERT.CurrencyApp.WalletService.Tests.Api;

public class WalletControllerTests
{
    private readonly Mock<InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher.ICommandDispatcher> _mockCommandDispatcher;
    private readonly Mock<InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher.IQueryDispatcher> _mockQueryDispatcher;
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
        _mockCommandDispatcher = new Mock<InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher.ICommandDispatcher>();
        _mockQueryDispatcher = new Mock<InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher.IQueryDispatcher>();
        _controller = new WalletController(_mockCommandDispatcher.Object, _mockQueryDispatcher.Object);
    }

    [Fact]
    public async Task CreateWallet_ReturnsCreatedResult_WithWalletId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateWalletRequest { Name = "My Wallet" };
        var walletId = Guid.NewGuid();

        _mockCommandDispatcher
            .Setup(m => m.SendAsync<CreateWalletCommand, Guid>(It.IsAny<CreateWalletCommand>(), default))
            .ReturnsAsync(walletId);

        // Act
        var result = await _controller.CreateWallet(userId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(WalletController.GetWalletBalances), createdResult.ActionName);
        Assert.Equal(walletId, createdResult.Value);
    }

    [Fact]
    public async Task GetWalletBalances_ReturnsOk_WithBalances()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var balances = new List<WalletBalanceDto>
        {
            new() { CurrencyCode = "TOP", Amount = 100 },
            new() { CurrencyCode = "MOP", Amount = 50 }
        };

        _mockQueryDispatcher
            .Setup(m => m.QueryAsync<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>(It.IsAny<GetWalletBalancesQuery>(), default))
            .ReturnsAsync(balances);

        // Act
        var result = await _controller.GetWalletBalances(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBalances = Assert.IsAssignableFrom<IReadOnlyCollection<WalletBalanceDto>>(okResult.Value);
        Assert.Equal(2, returnedBalances.Count);
    }
}
