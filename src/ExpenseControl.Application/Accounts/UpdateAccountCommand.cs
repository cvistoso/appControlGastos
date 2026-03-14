using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    AccountType Type,
    string Currency,
    decimal InitialBalance,
    bool IsActive) : IRequest<Result<AccountDto>>;
