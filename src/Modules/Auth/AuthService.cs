using System.Security.Cryptography;
using System.Text;
using Banana.Auth.Api.Common.Exceptions;
using Banana.Auth.Api.Infrastructure.Security;
using Banana.Auth.Api.Modules.Users;
using Banana.Auth.Api.Modules.Users.DTOs;
using Banana.Auth.Api.Modules.Auth.DTOs;

namespace Banana.Auth.Api.Modules.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new ConflictException("Email already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        var refreshToken = SetRefreshToken(user);
        await _userRepository.AddAsync(user);

        return BuildAuthResponse(user, refreshToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException();
        }

        var refreshToken = SetRefreshToken(user);
        await _userRepository.UpdateAsync(user);

        return BuildAuthResponse(user, refreshToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedException("Refresh token is required.");
        }

        var refreshTokenHash = HashRefreshToken(request.RefreshToken);

        var user = await _userRepository.GetByRefreshTokenHashAsync(refreshTokenHash);

        if (user is null || user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var newRefreshToken = SetRefreshToken(user);
        await _userRepository.UpdateAsync(user);

        return BuildAuthResponse(user, newRefreshToken);
    }

    private string SetRefreshToken(User user)
    {
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenHash = HashRefreshToken(refreshToken);
        user.RefreshTokenCreatedAt = DateTime.UtcNow;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        return refreshToken;
    }

    private AuthResponse BuildAuthResponse(User user, string refreshToken)
    {
        return new AuthResponse(
            _jwtService.GenerateAccessToken(user),
            refreshToken,
            new UserResponse(user.Id, user.Email, user.CreatedAt)
        );
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }
}
