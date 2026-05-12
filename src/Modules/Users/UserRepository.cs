using Banana.Auth.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Banana.Auth.Api.Modules.Users;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.RefreshTokenHash == refreshTokenHash);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}
