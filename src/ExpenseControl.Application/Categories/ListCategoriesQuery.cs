using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public record ListCategoriesQuery(
    int Page = 1,
    int PageSize = 20,
    CategoryType? Type = null,
    bool IncludeInactive = false) : IRequest<Result<PagedResult<CategoryDto>>>;
