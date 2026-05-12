using Banana.Auth.Api.Common.Exceptions;
using Banana.Auth.Api.Modules.Users.DTOs;

namespace Banana.Auth.Api.Modules.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new AppException("User not found.", 404);
        }

        return new UserResponse(user.Id, user.Email, user.CreatedAt);
    }
}
