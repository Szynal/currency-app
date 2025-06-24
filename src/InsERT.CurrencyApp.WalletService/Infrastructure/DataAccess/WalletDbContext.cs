using Microsoft.EntityFrameworkCore;
using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletBalance> WalletBalances => Set<WalletBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
    }
}
