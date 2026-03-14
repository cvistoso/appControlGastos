using ExpenseControl.Application.Interfaces;
using MediatR;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Auth;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.Email, request.Password, null, cancellationToken);
    }
}
