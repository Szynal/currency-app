using Microsoft.EntityFrameworkCore;
using InsERT.CurrencyApp.WalletService.DataAccess;

namespace InsERT.CurrencyApp.WalletService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<WalletDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("WalletDb")));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
            dbContext.Database.Migrate(); 
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
