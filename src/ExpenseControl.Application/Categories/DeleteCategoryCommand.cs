using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
