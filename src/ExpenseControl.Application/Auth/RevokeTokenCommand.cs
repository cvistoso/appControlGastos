using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Auth;

public record RevokeTokenCommand(string RefreshToken, string? IpAddress) : IRequest<Result>;
