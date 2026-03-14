using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public class GetAccountQueryHandler(IAccountRepository repository, IUserContext userContext)
    : IRequestHandler<GetAccountQuery, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<AccountDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var account = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (account is null)
            return Result<AccountDto>.Failure(ErrorCodes.NotFound, "Account not found.");
        return Result<AccountDto>.Success(AccountMapping.ToDto(account));
    }
}
