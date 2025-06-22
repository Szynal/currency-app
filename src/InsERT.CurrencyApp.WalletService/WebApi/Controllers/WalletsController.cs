using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Wallets.Models;
using InsERT.CurrencyApp.WalletService.DataAccess;
using InsERT.CurrencyApp.WalletService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.WalletService.WebApi.Controllers;

[ApiController]
[Route("wallets")]
public class WalletsController(WalletDbContext db) : ControllerBase
{
    private readonly WalletDbContext _db = db;

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Wallet name is required.");

        var wallet = new Wallet(request.Name);

        await _db.Wallets.AddAsync(wallet);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWallets), new { id = wallet.Id }, wallet.Id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WalletDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWallets()
    {
        var wallets = await _db.Wallets
            .Where(w => !w.IsDeleted)
            .Include(w => w.Balances)
            .ToListAsync();

        var result = wallets.Select(w => new WalletDto
        {
            Id = w.Id,
            Name = w.Name,
            Balances = w.Balances
                .Select(b => new WalletBalanceDto
                {
                    CurrencyCode = b.CurrencyCode,
                    Amount = b.Amount
                }).ToList()
        });

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == id);

        if (wallet is null || wallet.IsDeleted)
            return NotFound();

        wallet.Delete();
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
