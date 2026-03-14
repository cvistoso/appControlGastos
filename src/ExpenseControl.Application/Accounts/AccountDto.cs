using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.Accounts;

public record AccountDto(
    Guid Id,
    string Name,
    AccountType Type,
    string Currency,
    decimal InitialBalance,
    bool IsActive,
    DateTime CreatedAtUtc);
