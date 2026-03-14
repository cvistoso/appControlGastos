using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Application.Categories;

public static class CategoryMapping
{
    public static CategoryDto ToDto(Category c) => new(
        c.Id, c.Name, c.Description, c.Type, c.Color, c.Icon, c.SortOrder, c.IsActive, c.ParentCategoryId, c.CreatedAtUtc);
}
