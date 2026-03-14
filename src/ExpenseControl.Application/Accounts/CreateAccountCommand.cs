using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public record CreateAccountCommand(
    string Name,
    AccountType Type,
    string Currency,
    decimal InitialBalance) : IRequest<Result<AccountDto>>;
