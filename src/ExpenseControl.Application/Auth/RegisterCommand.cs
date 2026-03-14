using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Auth;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? IpAddress = null) : IRequest<Result<AuthResponse>>;
