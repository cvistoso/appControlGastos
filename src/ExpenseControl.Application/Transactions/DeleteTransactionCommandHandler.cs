using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public class DeleteTransactionCommandHandler(ITransactionRepository repository, IUserContext userContext)
    : IRequestHandler<DeleteTransactionCommand, Result>
{
    public async Task<Result> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var deleted = await repository.SoftDeleteAsync(userContext.UserId.Value, request.Id, cancellationToken);
        return deleted ? Result.Success() : Result.Failure(ErrorCodes.NotFound, "Transaction not found.");
    }
}
