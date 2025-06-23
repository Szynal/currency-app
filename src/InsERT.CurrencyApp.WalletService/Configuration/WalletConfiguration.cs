using InsERT.CurrencyApp.WalletService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .IsRequired();

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(w => w.Balances)
            .WithOne(b => b.Wallet)
            .HasForeignKey(b => b.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(w => w.Balances)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
