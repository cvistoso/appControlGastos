using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryDto>>;
