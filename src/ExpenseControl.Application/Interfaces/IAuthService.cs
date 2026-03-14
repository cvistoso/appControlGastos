using ExpenseControl.Application.Auth;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(string firstName, string lastName, string email, string password, string? ipAddress, CancellationToken cancellationToken = default);
}
