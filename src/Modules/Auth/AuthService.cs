using Banana.Auth.Api.Modules.Auth.DTOs;

namespace Banana.Auth.Api.Modules.Auth;

public class AuthService : IAuthService
{
    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}
