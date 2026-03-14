using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public class UpdateTransactionCommandHandler(ITransactionRepository repository, IUserContext userContext)
    : IRequestHandler<UpdateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<TransactionDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var transaction = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (transaction is null)
            return Result<TransactionDto>.Failure(ErrorCodes.NotFound, "Transaction not found.");
        transaction.AccountId = request.AccountId;
        transaction.TransferAccountId = request.TransferAccountId;
        transaction.CategoryId = request.CategoryId;
        transaction.Type = request.Type;
        transaction.Description = request.Description.Trim();
        transaction.Amount = request.Amount;
        transaction.Currency = request.Currency.ToUpperInvariant();
        transaction.TransactionDateUtc = request.TransactionDateUtc.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(request.TransactionDateUtc, DateTimeKind.Utc) : request.TransactionDateUtc.ToUniversalTime();
        transaction.ValueDateUtc = request.ValueDateUtc.HasValue ? (request.ValueDateUtc.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(request.ValueDateUtc.Value, DateTimeKind.Utc) : request.ValueDateUtc.Value.ToUniversalTime()) : null;
        transaction.Notes = request.Notes?.Trim();
        transaction.PaymentMethod = request.PaymentMethod?.Trim();
        transaction.Merchant = request.Merchant?.Trim();
        transaction.IsReconciled = request.IsReconciled;
        transaction.UpdatedAtUtc = DateTime.UtcNow;
        transaction.UpdatedByUserId = userContext.UserId;
        await repository.UpdateAsync(transaction, cancellationToken);
        return Result<TransactionDto>.Success(TransactionMapping.ToDto(transaction));
    }
}
