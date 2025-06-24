using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;

public class CreateWithdrawCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    IWalletServiceClient walletServiceClient,
    ILogger<CreateWithdrawCommandHandler> logger
) : ICommandHandler<CreateWithdrawCommand, Unit>
{
    public async Task<Unit> HandleAsync(CreateWithdrawCommand command, CancellationToken cancellationToken = default)
    {
        var transaction = Transaction.CreateWithdrawal(command.WalletId, command.CurrencyCode, command.Amount);

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await walletServiceClient.ApplyTransactionAsync(transaction, cancellationToken);

        transaction.MarkAccepted();
        await transactionRepository.UpdateAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Withdraw transaction {TransactionId} processed.", transaction.Id);

        return Unit.Value;
    }
}
