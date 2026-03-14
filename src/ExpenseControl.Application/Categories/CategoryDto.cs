using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    CategoryType Type,
    string Color,
    string? Icon,
    int SortOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    DateTime CreatedAtUtc);
