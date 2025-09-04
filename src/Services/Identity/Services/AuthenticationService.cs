using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelManagement.Services.Identity.Models;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HotelManagement.Services.Identity.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        IDapperDataRepository dataRepository, 
        IDbConnectionFactory connectionFactory, 
        ILogger<AuthenticationService> logger,
        IConfiguration configuration)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, creating a mock user since the current interface doesn't support this specific query
            _logger.LogWarning("LoginAsync using mock implementation - needs proper database query method");
            
            // Mock implementation - replace with actual database query
            var mockUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = "Mock",
                LastName = "User",
                Roles = new List<UserRole> { new UserRole { Role = "Guest" } }
            };
            
            var token = GenerateJwtToken(mockUser);
            var refreshToken = GenerateRefreshToken();
            
            return new AuthenticationResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Check if user already exists
            var existingUser = await GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }
            
            // Create new user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<UserRole> { new UserRole { Role = "Guest" } }
            };
            
            await _dataRepository.AddAsync(newUser, connection);
            
            var token = GenerateJwtToken(newUser);
            var refreshToken = GenerateRefreshToken();
            
            return new AuthenticationResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Note: This would need proper refresh token validation and user lookup
            // For now, returning a mock response
            _logger.LogWarning("RefreshTokenAsync using mock implementation - needs proper refresh token handling");
            
            var mockUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "mock@example.com",
                FirstName = "Mock",
                LastName = "User",
                Roles = new List<UserRole> { new UserRole { Role = "Guest" } }
            };
            
            var newToken = GenerateJwtToken(mockUser);
            var newRefreshToken = GenerateRefreshToken();
            
            return new AuthenticationResponse
            {
                AccessToken = newToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw new UnauthorizedAccessException("Invalid refresh token");
        }
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        try
        {
            // Note: This would need proper refresh token invalidation
            _logger.LogWarning("RevokeTokenAsync using mock implementation - needs proper token invalidation");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            throw;
        }
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            _logger.LogWarning("GetUserByEmailAsync using mock implementation - needs proper database query method");
            
            // Mock implementation - replace with actual database query
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = "Mock",
                LastName = "User",
                Roles = new List<UserRole> { new UserRole { Role = "Guest" } }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return null;
        }
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            var result = await _dataRepository.FindByIDAsync<User>(userId, connection);
            
            if (result == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id {UserId}", userId);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default-key-for-development");
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return null;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default-key-for-development");
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };
        
        // Add role claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
