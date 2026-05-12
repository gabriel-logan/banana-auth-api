using Banana.Auth.Api.Modules.Users.DTOs;

namespace Banana.Auth.Api.Modules.Users;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);
}
