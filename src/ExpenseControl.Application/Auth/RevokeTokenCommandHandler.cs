using ExpenseControl.Application.Interfaces;
using MediatR;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Auth;

public class RevokeTokenCommandHandler(IAuthService authService) : IRequestHandler<RevokeTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RevokeTokenAsync(request.RefreshToken, request.IpAddress, cancellationToken);
    }
}
