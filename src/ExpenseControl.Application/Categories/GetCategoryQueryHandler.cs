using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public class GetCategoryQueryHandler(ICategoryRepository repository, IUserContext userContext)
    : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<CategoryDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var category = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Failure(ErrorCodes.NotFound, "Category not found.");
        return Result<CategoryDto>.Success(CategoryMapping.ToDto(category));
    }
}
