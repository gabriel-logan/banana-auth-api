using Microsoft.EntityFrameworkCore;

namespace Banana.Auth.Api.Infrastructure.Database;

public class AuthDbInitializer
{
    private readonly AuthDbContext _dbContext;
    private readonly AuthDbSeeder _seeder;

    public AuthDbInitializer(AuthDbContext dbContext, AuthDbSeeder seeder)
    {
        _dbContext = dbContext;
        _seeder = seeder;
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
            """);

        await _dbContext.Database.MigrateAsync();
        await _seeder.SeedAsync();
    }
}
