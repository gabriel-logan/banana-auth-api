using Banana.Auth.Api.Infrastructure.Security;
using Banana.Auth.Api.Modules.Users;
using Microsoft.EntityFrameworkCore;

namespace Banana.Auth.Api.Infrastructure.Database;

public class AuthDbSeeder
{
    private readonly AuthDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthDbSeeder> _logger;

    public AuthDbSeeder(
        AuthDbContext dbContext,
        IPasswordHasher passwordHasher,
        ILogger<AuthDbSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Users.AnyAsync())
        {
            _logger.LogDebug("Skipping auth seed because users already exist.");
            return;
        }

        const string defaultPassword = "Banana#2026";

        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "ana.silva@banana.local",
                PasswordHash = _passwordHasher.Hash(defaultPassword)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "bruno.costa@banana.local",
                PasswordHash = _passwordHasher.Hash(defaultPassword)
            }
        };

        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Auth seed created with {UserCount} users.",
            users.Length);
    }
}
