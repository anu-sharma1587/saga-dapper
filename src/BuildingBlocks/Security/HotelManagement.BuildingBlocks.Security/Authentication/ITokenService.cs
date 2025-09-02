using System.Security.Claims;

namespace HotelManagement.BuildingBlocks.Security.Authentication;

public interface ITokenService
{
    Task<TokenResponse> GenerateAccessTokenAsync(string userId, string email, IEnumerable<string> roles);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    ClaimsPrincipal? ValidateToken(string token);
}
