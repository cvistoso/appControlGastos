using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public class CreateAccountCommandHandler(IAccountRepository repository, IUserContext userContext)
    : IRequestHandler<CreateAccountCommand, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<AccountDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userContext.UserId.Value,
            Name = request.Name.Trim(),
            Type = request.Type,
            Currency = request.Currency.ToUpperInvariant(),
            InitialBalance = request.InitialBalance,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        var created = await repository.AddAsync(account, cancellationToken);
        return Result<AccountDto>.Success(AccountMapping.ToDto(created));
    }
}
