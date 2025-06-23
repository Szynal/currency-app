using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;

public class WalletDbContextFactory : IDesignTimeDbContextFactory<WalletDbContext>
{
    public WalletDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("WalletDb")
            ?? configuration["AppSettings:WalletDbConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("WalletDb connection string not found in configuration.");

        var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>()
            .UseNpgsql(connectionString);

        return new WalletDbContext(optionsBuilder.Options);
    }
}
