namespace InsERT.CurrencyApp.CurrencyService.Configuration;

public class AppSettings(IConfiguration config)
{
    public string ConnectionString { get; } = config.GetConnectionString("Postgres")
            ?? throw new ArgumentNullException("ConnectionStrings:Postgres", "Missing connection string 'Postgres' in configuration.");
    public int FetchIntervalMinutes { get; } = config.GetValue<int?>("FetchIntervalMinutes")
            ?? throw new ArgumentNullException("FetchIntervalMinutes", "Missing 'FetchIntervalMinutes' in configuration.");
}
