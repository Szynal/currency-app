using InsERT.CurrencyApp.CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
namespace InsERT.CurrencyApp.CurrencyService.Infrastructure;


public class CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : DbContext(options)
{
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(e => new { e.Code, e.EffectiveDate })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
