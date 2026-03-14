using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public class UpdateAccountCommandHandler(IAccountRepository repository, IUserContext userContext)
    : IRequestHandler<UpdateAccountCommand, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<AccountDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var account = await repository.GetByIdAsync(userContext.UserId.Value, request.Id, cancellationToken);
        if (account is null)
            return Result<AccountDto>.Failure(ErrorCodes.NotFound, "Account not found.");
        account.Name = request.Name.Trim();
        account.Type = request.Type;
        account.Currency = request.Currency.ToUpperInvariant();
        account.InitialBalance = request.InitialBalance;
        account.IsActive = request.IsActive;
        account.UpdatedAtUtc = DateTime.UtcNow;
        await repository.UpdateAsync(account, cancellationToken);
        return Result<AccountDto>.Success(AccountMapping.ToDto(account));
    }
}
