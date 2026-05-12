using Banana.Auth.Api.Modules.Users.DTOs;

namespace Banana.Auth.Api.Modules.Users;

public class UserService : IUserService
{
    public Task<UserResponse> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
