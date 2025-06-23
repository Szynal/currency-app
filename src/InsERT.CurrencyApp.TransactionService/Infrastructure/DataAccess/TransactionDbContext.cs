using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.WalletId).IsRequired();
            builder.Property(t => t.Type).IsRequired();
            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.Amount)
                   .HasPrecision(18, 4)
                   .IsRequired();

            builder.Property(t => t.CurrencyCode)
                   .HasMaxLength(3)
                   .IsFixedLength(true)
                   .IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired();

            builder.Property(t => t.ProcessedAt);

            builder.Property(t => t.ConvertedAmount)
                   .HasPrecision(18, 4);

            builder.Property(t => t.ConvertedCurrencyCode)
                   .HasMaxLength(3)
                   .IsFixedLength(true);
        });
    }
}
