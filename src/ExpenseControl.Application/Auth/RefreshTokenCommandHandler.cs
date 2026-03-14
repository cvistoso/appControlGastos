using ExpenseControl.Application.Interfaces;
using MediatR;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Auth;

public class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(request.RefreshToken, request.IpAddress, cancellationToken);
    }
}
