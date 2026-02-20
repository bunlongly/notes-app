using Dapper;
using Microsoft.IdentityModel.Tokens;
using NotesAPI.Data;
using NotesAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NotesAPI.Services;

public class UserService : IUserService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;

    public UserService(IDbConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<User> CreateAsync(RegisterRequest request)
    {
        var existingUser = await GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new Exception("User already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Users (Email, PasswordHash, FullName, CreatedAt) 
                    VALUES (@Email, @PasswordHash, @FullName, @CreatedAt);
                    SELECT last_insert_rowid()";

        var userId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        });

        return new User
        {
            Id = userId,
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
    {
        var user = await GetByEmailAsync(request.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("Invalid email or password");
        }

        // Generate both access and refresh tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateAndStoreRefreshTokenAsync(user.Id);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Validate refresh token from database
        var storedToken = await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "SELECT * FROM RefreshTokens WHERE Token = @Token",
            new { Token = refreshToken });

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token");
        }

        // Get user associated with token
        var user = await GetByIdAsync(storedToken.UserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Delete old refresh token for security (token rotation)
        await connection.ExecuteAsync(
            "DELETE FROM RefreshTokens WHERE Token = @Token",
            new { Token = refreshToken });

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user.Id);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    // Generate short-lived access token (15 minutes)
    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Generate and store long-lived refresh token (7 days)
    private async Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Clean up expired tokens for this user
        await connection.ExecuteAsync(
            "DELETE FROM RefreshTokens WHERE UserId = @UserId AND ExpiresAt < @Now",
            new { UserId = userId, Now = DateTime.UtcNow });

        // Generate secure random refresh token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        // Store refresh token in database
        var sql = @"INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt) 
                    VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt);
                    SELECT last_insert_rowid()";

        refreshToken.Id = await connection.ExecuteScalarAsync<int>(sql, refreshToken);

        return refreshToken;
    }
}
