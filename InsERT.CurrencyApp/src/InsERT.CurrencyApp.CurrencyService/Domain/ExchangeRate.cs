namespace InsERT.CurrencyApp.CurrencyService.Domain;

public class ExchangeRate
{
    public int Id { get; set; }
    public string Currency { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal Rate { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
