using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public class GetTransactionQueryHandler(ITransactionRepository repository, IUserContext userContext)
    : IRequestHandler<GetTransactionQuery, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<TransactionDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var transaction = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (transaction is null)
            return Result<TransactionDto>.Failure(ErrorCodes.NotFound, "Transaction not found.");
        return Result<TransactionDto>.Success(TransactionMapping.ToDto(transaction));
    }
}
