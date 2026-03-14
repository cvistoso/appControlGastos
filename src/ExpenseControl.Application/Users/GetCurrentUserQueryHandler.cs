using ExpenseControl.Application.Auth; // UserDto
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Users;

public class GetCurrentUserQueryHandler(IUserContext userContext, IUserRepository userRepository)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<UserDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");

        var user = await userRepository.GetByIdAsync(userContext.UserId.Value, cancellationToken);
        if (user is null)
            return Result<UserDto>.Failure(ErrorCodes.NotFound, "User not found.");

        return Result<UserDto>.Success(new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString()));
    }
}
