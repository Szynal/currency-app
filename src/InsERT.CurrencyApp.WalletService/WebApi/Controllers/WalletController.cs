using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Queries;
using InsERT.CurrencyApp.WalletService.WebApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.WalletService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ILogger<WalletController> _logger;

    public WalletController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<WalletController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    [HttpPost("{userId}/wallets")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWallet(Guid userId, [FromBody] CreateWalletRequest request)
    {
        var command = new CreateWalletCommand(userId, request.Name);
        var walletId = await _commandDispatcher.SendAsync<CreateWalletCommand, Guid>(command);

        return CreatedAtAction(nameof(GetWalletBalances), new { userId }, walletId);
    }

    [HttpGet("{userId}/balances")]
    [ProducesResponseType(typeof(IEnumerable<WalletBalanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWalletBalances(Guid userId)
    {
        var query = new GetWalletBalancesQuery(userId);
        var balances = await _queryDispatcher.QueryAsync<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>(query);

        return Ok(balances);
    }

    [HttpPost("{walletId}/balances")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddBalance(Guid walletId, [FromBody] AddBalanceRequest request)
    {
        var command = new AddWalletBalanceCommand(walletId, request.CurrencyCode, request.InitialAmount);
        await _commandDispatcher.SendAsync<AddWalletBalanceCommand, Unit>(command);
        return Ok();
    }

    [HttpPost("apply-transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApplyTransaction([FromBody] ApplyTransactionRequest request)
    {
        _logger.LogInformation("Received ApplyTransaction: {@Request}", request);

        try
        {
            var command = new ApplyWalletTransactionCommand(
                request.WalletId,
                request.TransactionId,
                request.Type,
                request.Amount,
                request.CurrencyCode,
                request.ConvertedAmount,
                request.ConvertedCurrencyCode
            );

            await _commandDispatcher.SendAsync<ApplyWalletTransactionCommand, Unit>(command);

            _logger.LogInformation("Transaction applied successfully. WalletId={WalletId}, TxId={TransactionId}",
                request.WalletId, request.TransactionId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction application failed. WalletId={WalletId}, TxId={TransactionId}",
                request.WalletId, request.TransactionId);

            return BadRequest(new
            {
                Error = "Transaction failed.",
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}
