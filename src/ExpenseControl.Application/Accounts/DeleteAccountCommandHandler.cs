using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public class DeleteAccountCommandHandler(IAccountRepository repository, IUserContext userContext)
    : IRequestHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var deleted = await repository.SoftDeleteAsync(userContext.UserId.Value, request.Id, cancellationToken);
        return deleted ? Result.Success() : Result.Failure(ErrorCodes.NotFound, "Account not found.");
    }
}
