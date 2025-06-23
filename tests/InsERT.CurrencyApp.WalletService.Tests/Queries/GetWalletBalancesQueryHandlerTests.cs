using InsERT.CurrencyApp.WalletService.Application.Queries;
using InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;
using InsERT.CurrencyApp.WalletService.Domain.Entities;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;
using Moq;

namespace InsERT.CurrencyApp.WalletService.Tests.Queries;

public class GetWalletBalancesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenWalletsHaveNoBalances()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var emptyWallet = Wallet.Create(userId, "Empty Wallet");

        var mockRepo = new Mock<IWalletRepository>();
        mockRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([emptyWallet]);

        var query = new GetWalletBalancesQuery(userId);
        var handler = new GetWalletBalancesQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}
