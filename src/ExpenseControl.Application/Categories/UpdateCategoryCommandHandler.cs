using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public class UpdateCategoryCommandHandler(ICategoryRepository repository, IUserContext userContext)
    : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<CategoryDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var category = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Failure(ErrorCodes.NotFound, "Category not found.");
        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.Type = request.Type;
        category.Color = string.IsNullOrEmpty(request.Color) ? category.Color : request.Color.Trim();
        category.Icon = request.Icon?.Trim();
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAtUtc = DateTime.UtcNow;
        await repository.UpdateAsync(category, cancellationToken);
        return Result<CategoryDto>.Success(CategoryMapping.ToDto(category));
    }
}
