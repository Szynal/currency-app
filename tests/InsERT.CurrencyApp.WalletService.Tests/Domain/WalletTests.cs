using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Tests.Domain;

public class WalletTests
{
    [Fact]
    public void Create_ShouldInitializeWalletWithCorrectData()
    {
        var userId = Guid.NewGuid();
        var name = "Egzotyczny Portfel";

        var wallet = Wallet.Create(userId, name);

        Assert.Equal(userId, wallet.UserId);
        Assert.Equal(name, wallet.Name);
        Assert.Empty(wallet.Balances);
        Assert.NotEqual(Guid.Empty, wallet.Id);
    }

    [Fact]
    public void ApplyDeposit_ShouldAddNewBalance_WhenCurrencyIsNew()
    {
        var wallet = Wallet.Create(Guid.NewGuid(), "Makau Portfel");

        wallet.ApplyDeposit("MOP", 123.45m);

        var balance = wallet.Balances.Single(b => b.CurrencyCode == "MOP");
        Assert.Equal(123.45m, balance.Amount);
    }

    [Fact]
    public void ApplyDeposit_ShouldIncreaseBalance_WhenCurrencyExists()
    {
        var wallet = Wallet.Create(Guid.NewGuid(), "Argentyński Portfel");
        wallet.ApplyDeposit("ARS", 100m);

        wallet.ApplyDeposit("ARS", 50m);

        var balance = wallet.Balances.Single(b => b.CurrencyCode == "ARS");
        Assert.Equal(150m, balance.Amount);
    }

    [Fact]
    public void ApplyWithdrawal_ShouldDecreaseBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid(), "Tadżycki Portfel");
        wallet.ApplyDeposit("TJS", 500m);

        wallet.ApplyWithdrawal("TJS", 125m);

        var balance = wallet.Balances.Single(b => b.CurrencyCode == "TJS");
        Assert.Equal(375m, balance.Amount);
    }

    [Fact]
    public void ApplyWithdrawal_ShouldThrow_WhenInsufficientFunds()
    {
        var wallet = Wallet.Create(Guid.NewGuid(), "Kambodża Portfel");
        wallet.ApplyDeposit("KHR", 20m); 

        Assert.Throws<InvalidOperationException>(() => wallet.ApplyWithdrawal("KHR", 100m));
    }

    [Fact]
    public void ApplyConversion_ShouldWithdrawAndDeposit()
    {
        var wallet = Wallet.Create(Guid.NewGuid(), "Zimbabwe i Nepal");
        wallet.ApplyDeposit("ZWG", 10m); 

        wallet.ApplyConversion("ZWG", 5m, "NPR", 134.25m); 

        var zwg = wallet.Balances.Single(b => b.CurrencyCode == "ZWG");
        var npr = wallet.Balances.Single(b => b.CurrencyCode == "NPR");

        Assert.Equal(5m, zwg.Amount);
        Assert.Equal(134.25m, npr.Amount);
    }
}
