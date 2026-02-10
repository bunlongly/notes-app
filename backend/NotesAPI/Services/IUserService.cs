using NotesAPI.Models;

namespace NotesAPI.Services;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(RegisterRequest request);
    Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
}
