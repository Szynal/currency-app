using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Tests.Domain;

public class WalletBalanceTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var walletId = Guid.NewGuid();
        var balance = new WalletBalance(walletId, "TOP", 12.34m); 

        Assert.Equal(walletId, balance.WalletId);
        Assert.Equal("TOP", balance.CurrencyCode);
        Assert.Equal(12.34m, balance.Amount);
    }

    [Fact]
    public void Increase_ShouldAddAmount()
    {
        var balance = new WalletBalance(Guid.NewGuid(), "RUB", 10m); 

        balance.Increase(5m);

        Assert.Equal(15m, balance.Amount);
    }

    [Fact]
    public void Increase_ShouldThrow_WhenAmountIsZeroOrNegative()
    {
        var balance = new WalletBalance(Guid.NewGuid(), "SCR", 10m); 

        Assert.Throws<InvalidOperationException>(() => balance.Increase(0));
        Assert.Throws<InvalidOperationException>(() => balance.Increase(-5));
    }

    [Fact]
    public void Decrease_ShouldSubtractAmount()
    {
        var balance = new WalletBalance(Guid.NewGuid(), "OMR", 50m); 

        balance.Decrease(20m);

        Assert.Equal(30m, balance.Amount);
    }

    [Fact]
    public void Decrease_ShouldThrow_WhenAmountExceedsBalance()
    {
        var balance = new WalletBalance(Guid.NewGuid(), "MNT", 10m); 

        Assert.Throws<InvalidOperationException>(() => balance.Decrease(100m));
    }

    [Fact]
    public void Decrease_ShouldThrow_WhenAmountIsZeroOrNegative()
    {
        var balance = new WalletBalance(Guid.NewGuid(), "PKR", 10m); 

        Assert.Throws<InvalidOperationException>(() => balance.Decrease(0));
        Assert.Throws<InvalidOperationException>(() => balance.Decrease(-1));
    }
}
