using System.Linq;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public class ListTransactionsQueryHandler(ITransactionRepository repository, IUserContext userContext)
    : IRequestHandler<ListTransactionsQuery, Result<PagedResult<TransactionDto>>>
{
    public async Task<Result<PagedResult<TransactionDto>>> Handle(ListTransactionsQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<PagedResult<TransactionDto>>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var paged = await repository.GetPagedAsync(
            userContext.UserId.Value, page, pageSize,
            request.Type, request.AccountId, request.CategoryId, request.FromDate, request.ToDate,
            cancellationToken);
        var dtos = paged.Items.Select(TransactionMapping.ToDto).ToList();
        return Result<PagedResult<TransactionDto>>.Success(new PagedResult<TransactionDto>(dtos, paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages));
    }
}
