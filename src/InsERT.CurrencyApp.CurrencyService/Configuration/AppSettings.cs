namespace InsERT.CurrencyApp.CurrencyService.Configuration;

public class AppSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int FetchIntervalMinutes { get; set; }
    public NbpClientSettings NbpClient { get; set; } = new();
}

