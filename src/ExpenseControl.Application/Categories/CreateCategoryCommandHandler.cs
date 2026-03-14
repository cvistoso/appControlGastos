using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public class CreateCategoryCommandHandler(ICategoryRepository repository, IUserContext userContext)
    : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<CategoryDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userContext.UserId.Value,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Type = request.Type,
            Color = string.IsNullOrEmpty(request.Color) ? "#007bff" : request.Color.Trim(),
            Icon = request.Icon?.Trim(),
            SortOrder = request.SortOrder,
            IsActive = true,
            ParentCategoryId = request.ParentCategoryId,
            CreatedAtUtc = DateTime.UtcNow
        };
        var created = await repository.AddAsync(category, cancellationToken);
        return Result<CategoryDto>.Success(CategoryMapping.ToDto(created));
    }
}
