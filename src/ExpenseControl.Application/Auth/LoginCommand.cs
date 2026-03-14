using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Auth;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    UserDto User);

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role);
