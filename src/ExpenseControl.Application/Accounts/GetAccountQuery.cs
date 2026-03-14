using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public record GetAccountQuery(Guid Id) : IRequest<Result<AccountDto>>;
