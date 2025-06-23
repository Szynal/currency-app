using Microsoft.EntityFrameworkCore;
using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;

public class WalletDbContext(DbContextOptions<WalletDbContext> options) : DbContext(options)
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletBalance> WalletBalances => Set<WalletBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wallet>(builder =>
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.UserId)
                   .IsRequired();

            builder.HasMany(w => w.Balances)
                   .WithOne()
                   .HasForeignKey(b => b.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(w => w.Balances)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<WalletBalance>(builder =>
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CurrencyCode)
                   .IsRequired()
                   .HasMaxLength(3)
                   .IsFixedLength(true);

            builder.Property(b => b.Amount)
                   .IsRequired()
                   .HasPrecision(18, 4);

            builder.Property(b => b.WalletId)
                   .IsRequired();

            builder.HasIndex(b => new { b.WalletId, b.CurrencyCode })
                   .IsUnique(); 
        });
    }
}
