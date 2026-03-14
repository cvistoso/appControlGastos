using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Categories;

public class DeleteCategoryCommandHandler(ICategoryRepository repository, IUserContext userContext)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var deleted = await repository.SoftDeleteAsync(userContext.UserId.Value, request.Id, cancellationToken);
        return deleted ? Result.Success() : Result.Failure(ErrorCodes.NotFound, "Category not found.");
    }
}
