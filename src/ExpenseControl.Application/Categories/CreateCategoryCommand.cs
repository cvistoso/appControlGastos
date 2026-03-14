using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    CategoryType Type,
    string? Color,
    string? Icon,
    int SortOrder,
    Guid? ParentCategoryId) : IRequest<Result<CategoryDto>>;
