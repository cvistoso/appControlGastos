using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Auth;

public record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<Result<AuthResponse>>;
