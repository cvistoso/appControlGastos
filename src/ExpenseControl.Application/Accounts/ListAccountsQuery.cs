using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public record ListAccountsQuery(int Page = 1, int PageSize = 20, bool IncludeInactive = false) : IRequest<Result<PagedResult<AccountDto>>>;
