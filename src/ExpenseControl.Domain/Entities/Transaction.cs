using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid? TransferAccountId { get; set; }
    public Account? TransferAccount { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDateUtc { get; set; }
    public DateTime? ValueDateUtc { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Merchant { get; set; }
    public bool IsReconciled { get; set; }
    public bool IsRecurring { get; set; }
    public Guid? RecurringTransactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}
