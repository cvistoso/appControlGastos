using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Application.Accounts;

public static class AccountMapping
{
    public static AccountDto ToDto(Account a) => new(
        a.Id, a.Name, a.Type, a.Currency, a.InitialBalance, a.IsActive, a.CreatedAtUtc);
}
