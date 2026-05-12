namespace Banana.Auth.Api.Modules.Users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
