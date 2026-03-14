using System.Linq;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public class ListCategoriesQueryHandler(ICategoryRepository repository, IUserContext userContext)
    : IRequestHandler<ListCategoriesQuery, Result<PagedResult<CategoryDto>>>
{
    public async Task<Result<PagedResult<CategoryDto>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<PagedResult<CategoryDto>>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var paged = await repository.GetPagedAsync(userContext.UserId.Value, page, pageSize, request.Type, request.IncludeInactive, cancellationToken);
        var dtos = paged.Items.Select(CategoryMapping.ToDto).ToList();
        return Result<PagedResult<CategoryDto>>.Success(new PagedResult<CategoryDto>(dtos, paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages));
    }
}
