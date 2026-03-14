using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public record CreateTransactionCommand(
    Guid AccountId,
    Guid? TransferAccountId,
    Guid CategoryId,
    TransactionType Type,
    string Description,
    decimal Amount,
    string Currency,
    DateTime TransactionDateUtc,
    DateTime? ValueDateUtc,
    string? Notes,
    string? PaymentMethod,
    string? Merchant) : IRequest<Result<TransactionDto>>;
