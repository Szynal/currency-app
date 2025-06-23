using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsERT.CurrencyApp.WalletService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;

    [HttpPost("{userId}/wallets")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWallet(Guid userId, [FromBody] CreateWalletRequest request)
    {
        var command = new CreateWalletCommand
        {
            UserId = userId,
            Name = request.Name
        };

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
}
