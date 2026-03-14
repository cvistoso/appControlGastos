using ExpenseControl.Application.Auth;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Users;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;
