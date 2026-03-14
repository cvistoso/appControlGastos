using System.Linq;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public class ListAccountsQueryHandler(IAccountRepository repository, IUserContext userContext)
    : IRequestHandler<ListAccountsQuery, Result<PagedResult<AccountDto>>>
{
    public async Task<Result<PagedResult<AccountDto>>> Handle(ListAccountsQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<PagedResult<AccountDto>>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var paged = await repository.GetPagedAsync(userContext.UserId.Value, page, pageSize, request.IncludeInactive, cancellationToken);
        var dtos = paged.Items.Select(AccountMapping.ToDto).ToList();
        return Result<PagedResult<AccountDto>>.Success(new PagedResult<AccountDto>(dtos, paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages));
    }
}
