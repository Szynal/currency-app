using InsERT.CurrencyApp.CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsERT.CurrencyApp.CurrencyService.DataAccess.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.Code, e.EffectiveDate })
               .IsUnique();

        builder.Property(e => e.Code)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(e => e.Currency)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.Rate)
               .HasPrecision(18, 4);

        builder.Property(e => e.EffectiveDate)
               .IsRequired();
    }
}
