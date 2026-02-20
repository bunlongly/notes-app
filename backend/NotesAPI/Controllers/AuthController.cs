using Microsoft.AspNetCore.Mvc;
using NotesAPI.Models;
using NotesAPI.Services;

namespace NotesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    // Check if cookies are enabled (best practice for production)
    private bool UseCookies => _configuration.GetValue<bool>("Auth:UseCookies");

    // Helper method to set tokens (localStorage or httpOnly cookies)
    private void SetTokens(AuthResponse authResponse)
    {
        if (UseCookies)
        {
            // Best Practice: httpOnly cookies (secure, can't be accessed by JavaScript)
            Response.Cookies.Append("accessToken", authResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,      // Prevents JavaScript access (XSS protection)
                Secure = true,        // Only sent over HTTPS in production
                SameSite = SameSiteMode.Strict,  // CSRF protection
                Expires = DateTimeOffset.Now.AddMinutes(15)
            });

            Response.Cookies.Append("refreshToken", authResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            Console.WriteLine("[Auth] Tokens set as httpOnly cookies (secure)");
        }
        else
        {
            // Development: tokens sent in response body for localStorage
            Console.WriteLine("[Auth] Tokens sent in response body for localStorage");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            Console.WriteLine($"[Register] Attempting to register user: {request.Email}");
            var user = await _userService.CreateAsync(request);
            Console.WriteLine($"[Register] User created successfully: {user.Id}");
            
            var authResponse = await _userService.AuthenticateAsync(new LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            });
            Console.WriteLine($"[Register] Authentication successful for: {request.Email}");

            SetTokens(authResponse);

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Register] ERROR: {ex.Message}");
            Console.WriteLine($"[Register] Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            Console.WriteLine($"[Login] Attempting login for: {request.Email}");
            var authResponse = await _userService.AuthenticateAsync(request);
            Console.WriteLine($"[Login] Login successful for: {request.Email}");
            
            SetTokens(authResponse);
            
            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Login] ERROR: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var authResponse = await _userService.RefreshTokenAsync(request.RefreshToken);
            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
