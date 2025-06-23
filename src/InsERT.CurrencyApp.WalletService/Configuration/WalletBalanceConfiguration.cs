using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess.Configurations;

public class WalletBalanceConfiguration : IEntityTypeConfiguration<WalletBalance>
{
    public void Configure(EntityTypeBuilder<WalletBalance> builder)
    {
        builder.ToTable("WalletBalances");

        builder.HasKey(wb => wb.Id);

        builder.Property(wb => wb.CurrencyCode)
               .HasColumnType("char(3)")
               .IsRequired();

        builder.Property(wb => wb.Amount)
               .HasColumnType("numeric(18,4)")
               .IsRequired();

        builder.HasIndex(wb => new { wb.WalletId, wb.CurrencyCode })
               .IsUnique();
    }
}
