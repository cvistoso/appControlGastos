using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Application.Transactions;

public static class TransactionMapping
{
    public static TransactionDto ToDto(Transaction t) => new(
        t.Id, t.AccountId, t.TransferAccountId, t.CategoryId, t.Type, t.Description, t.Amount, t.Currency,
        t.TransactionDateUtc, t.ValueDateUtc, t.Notes, t.PaymentMethod, t.Merchant, t.IsReconciled, t.CreatedAtUtc);
}
