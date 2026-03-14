using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    CategoryType Type,
    string? Color,
    string? Icon,
    int SortOrder,
    bool IsActive) : IRequest<Result<CategoryDto>>;
