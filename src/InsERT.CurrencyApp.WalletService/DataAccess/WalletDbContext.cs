using InsERT.CurrencyApp.WalletService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.WalletService.DataAccess;

public class WalletDbContext : DbContext
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletBalance> WalletBalances => Set<WalletBalance>();

    public WalletDbContext(DbContextOptions<WalletDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wallet>(builder =>
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
            builder.Property(w => w.IsDeleted).HasDefaultValue(false);

            builder.HasMany(w => w.Balances)
                   .WithOne()
                   .HasForeignKey(b => b.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(w => !w.IsDeleted);
        });

        modelBuilder.Entity<WalletBalance>(builder =>
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(b => b.Amount).HasPrecision(18, 4);
        });
    }

}
