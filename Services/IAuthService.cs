using ExpenseControlApp.Models;
using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<UserDto?> GetUserByIdAsync(int userId);
        string GenerateJwtToken(User user);
    }
}
