using Banana.Auth.Api.Modules.Users;

namespace Banana.Auth.Api.Infrastructure.Security;

public interface IJwtService
{
    string GenerateToken(User user);
}
