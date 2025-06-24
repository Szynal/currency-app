using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.WebApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ICommandDispatcher commandDispatcher, ILogger<TransactionController> logger) : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
    private readonly ILogger<TransactionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Deposit([FromBody] CreateDepositRequest request)
    {
        var command = new CreateDepositCommand(request.WalletId, request.CurrencyCode, request.Amount);
        await _commandDispatcher.SendAsync<CreateDepositCommand, Unit>(command);

        _logger.LogInformation("Deposit command sent for WalletId {WalletId}, Amount {Amount} {CurrencyCode}",
            request.WalletId, request.Amount, request.CurrencyCode);

        return Accepted();
    }

    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Withdraw([FromBody] CreateWithdrawRequest request)
    {
        var command = new CreateWithdrawCommand(request.WalletId, request.CurrencyCode, request.Amount);
        await _commandDispatcher.SendAsync<CreateWithdrawCommand, Unit>(command);

        _logger.LogInformation("Withdraw command sent for WalletId {WalletId}, Amount {Amount} {CurrencyCode}",
            request.WalletId, request.Amount, request.CurrencyCode);

        return Accepted();
    }

    [HttpPost("convert")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> ConvertCurrency([FromBody] CreateConvertRequest request)
    {
        var command = new CreateConversionCommand(
            request.WalletId,
            request.SourceCurrencyCode,
            request.SourceAmount,
            request.TargetCurrencyCode);

        await _commandDispatcher.SendAsync<CreateConversionCommand, Unit>(command);

        _logger.LogInformation("ConvertCurrency command sent for WalletId {WalletId}, SourceAmount {SourceAmount} {SourceCurrencyCode} to {TargetCurrencyCode}",
            request.WalletId, request.SourceAmount, request.SourceCurrencyCode, request.TargetCurrencyCode);

        return Accepted();
    }
}
