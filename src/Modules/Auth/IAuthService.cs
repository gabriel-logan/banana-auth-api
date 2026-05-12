using Banana.Auth.Api.Modules.Auth.DTOs;

namespace Banana.Auth.Api.Modules.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
