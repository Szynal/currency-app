using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.WebApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ICommandDispatcher commandDispatcher, ILogger<TransactionController> logger) : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
    private readonly ILogger<TransactionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] CreateDepositRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ApplyTransactionCommand(
            WalletId: request.WalletId,
            Type: Domain.Entities.TransactionType.Deposit,
            Amount: request.Amount,
            CurrencyCode: request.CurrencyCode);

        await _commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(command);

        _logger.LogInformation("Deposit command sent for WalletId {WalletId}, Amount {Amount} {CurrencyCode}",
            request.WalletId, request.Amount, request.CurrencyCode);

        return Accepted();
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] CreateWithdrawRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ApplyTransactionCommand(
            WalletId: request.WalletId,
            Type: Domain.Entities.TransactionType.Withdraw,
            Amount: request.Amount,
            CurrencyCode: request.CurrencyCode);

        await _commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(command);

        _logger.LogInformation("Withdraw command sent for WalletId {WalletId}, Amount {Amount} {CurrencyCode}",
            request.WalletId, request.Amount, request.CurrencyCode);

        return Accepted();
    }

    [HttpPost("convert")]
    public async Task<IActionResult> ConvertCurrency([FromBody] CreateConvertRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ApplyTransactionCommand(
            WalletId: request.WalletId,
            Type: Domain.Entities.TransactionType.ConvertCurrency,
            Amount: request.SourceAmount,
            CurrencyCode: request.SourceCurrencyCode,
            ConvertedAmount: request.TargetAmount,
            ConvertedCurrencyCode: request.TargetCurrencyCode);

        await _commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(command);

        _logger.LogInformation("ConvertCurrency command sent for WalletId {WalletId}, SourceAmount {SourceAmount} {SourceCurrencyCode} to TargetAmount {TargetAmount} {TargetCurrencyCode}",
            request.WalletId, request.SourceAmount, request.SourceCurrencyCode, request.TargetAmount, request.TargetCurrencyCode);

        return Accepted();
    }
}
