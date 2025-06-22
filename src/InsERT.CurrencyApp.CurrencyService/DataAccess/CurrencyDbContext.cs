using InsERT.CurrencyApp.CurrencyService.DataAccess.Configurations;
using InsERT.CurrencyApp.CurrencyService.Domain;

using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.DataAccess;

public class CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : DbContext(options)
{
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
