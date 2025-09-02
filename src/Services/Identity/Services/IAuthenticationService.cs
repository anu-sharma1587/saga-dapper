using System.Security.Claims;
using HotelManagement.Services.Identity.Models;

namespace HotelManagement.Services.Identity.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(Guid userId);
    ClaimsPrincipal? ValidateToken(string token);
}
