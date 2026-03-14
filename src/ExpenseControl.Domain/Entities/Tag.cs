namespace ExpenseControl.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
}
