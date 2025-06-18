namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

public class NbpTable
{
    public string Table { get; set; } = string.Empty;
    public string EffectiveDate { get; set; } = string.Empty;
    public List<NbpRate> Rates { get; set; } = [];
}

public class NbpRate
{
    public string Currency { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Mid { get; set; }
}
