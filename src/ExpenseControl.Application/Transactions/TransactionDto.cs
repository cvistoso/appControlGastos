using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.Transactions;

public record TransactionDto(
    Guid Id,
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
    string? Merchant,
    bool IsReconciled,
    DateTime CreatedAtUtc);
