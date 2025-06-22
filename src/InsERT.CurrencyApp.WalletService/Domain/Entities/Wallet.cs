using System;
using System.Collections.Generic;

namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    public List<WalletBalance> Balances { get; private set; } = new();
    public bool IsDeleted { get; private set; } = false;

    // EF Core constructor
    private Wallet() { }

    public Wallet(string name)
    {
        SetName(name);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Wallet name cannot be empty.", nameof(name));

        Name = name.Trim();
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Wallet is already deleted.");

        IsDeleted = true;
    }
}
