using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExpenseControl.Application.Auth;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Infrastructure.Data;
using ExpenseControl.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseControl.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const int RefreshTokenValidDays = 7;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        if (user is null)
            return Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure(ErrorCodes.Forbidden, "Account is disabled.");

        if (user.LockoutEndUtc.HasValue && user.LockoutEndUtc > DateTime.UtcNow)
            return Result<AuthResponse>.Failure(ErrorCodes.Forbidden, $"Account locked. Try again after {user.LockoutEndUtc:HH:mm} UTC.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
                user.LockoutEndUtc = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            user.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            return Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEndUtc = null;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);
        var (accessToken, expiresAt) = GenerateAccessToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            refreshToken.Token,
            expiresAt,
            refreshToken.ExpiresAtUtc,
            new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString())));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshTokenValue, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenValue, cancellationToken);

        if (token is null || !token.IsActive || token.User is null || !token.User.IsActive)
            return Result<AuthResponse>.Failure(ErrorCodes.TokenRevoked, "Invalid or expired refresh token.");

        token.RevokedAtUtc = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        var newRefreshToken = await CreateRefreshTokenAsync(token.UserId, ipAddress, cancellationToken);
        var (accessToken, expiresAt) = GenerateAccessToken(token.User);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            newRefreshToken.Token,
            expiresAt,
            newRefreshToken.ExpiresAtUtc,
            new UserDto(token.User.Id, token.User.FirstName, token.User.LastName, token.User.Email, token.User.Role.ToString())));
    }

    public async Task<Result> RevokeTokenAsync(string refreshTokenValue, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshTokenValue, cancellationToken);
        if (token is null)
            return Result.Success();

        token.RevokedAtUtc = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> RegisterAsync(string firstName, string lastName, string email, string password, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            return Result<AuthResponse>.Failure(ErrorCodes.Conflict, "Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12)),
            Role = UserRole.User,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);
        var (accessToken, expiresAt) = GenerateAccessToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            refreshToken.Token,
            expiresAt,
            refreshToken.ExpiresAtUtc,
            new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString())));
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(RefreshTokenValidDays),
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(cancellationToken);
        return token;
    }

    private (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var key = _config["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not set.");
        var issuer = _config["Jwt:Issuer"] ?? "ExpenseControl";
        var audience = _config["Jwt:Audience"] ?? "ExpenseControl";
        var expiryMinutes = int.TryParse(_config["Jwt:AccessTokenExpiryMinutes"], out var m) ? m : 15;

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return (accessToken, expiresAt);
    }
}
