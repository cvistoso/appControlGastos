using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public class CreateTransactionCommandHandler(ITransactionRepository repository, IUserContext userContext)
    : IRequestHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<TransactionDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userContext.UserId.Value,
            AccountId = request.AccountId,
            TransferAccountId = request.TransferAccountId,
            CategoryId = request.CategoryId,
            Type = request.Type,
            Description = request.Description.Trim(),
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            TransactionDateUtc = request.TransactionDateUtc.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(request.TransactionDateUtc, DateTimeKind.Utc) : request.TransactionDateUtc.ToUniversalTime(),
            ValueDateUtc = request.ValueDateUtc.HasValue ? (request.ValueDateUtc.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(request.ValueDateUtc.Value, DateTimeKind.Utc) : request.ValueDateUtc.Value.ToUniversalTime()) : null,
            Notes = request.Notes?.Trim(),
            PaymentMethod = request.PaymentMethod?.Trim(),
            Merchant = request.Merchant?.Trim(),
            IsReconciled = false,
            IsRecurring = false,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = userContext.UserId
        };
        var created = await repository.AddAsync(transaction, cancellationToken);
        return Result<TransactionDto>.Success(TransactionMapping.ToDto(created));
    }
}
