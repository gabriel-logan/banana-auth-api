using Banana.Auth.Api.Modules.Auth;
using Banana.Auth.Api.Modules.Users;
using Banana.Auth.Api.Infrastructure.Database;
using Banana.Auth.Api.Infrastructure.Security;
using Banana.Auth.Api.Common.Exceptions;
using Banana.Auth.Api.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration["DATABASE_URL"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => string.IsNullOrWhiteSpace(entry.Key) ? "body" : entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage)
                    .ToArray()
            );

        return new BadRequestObjectResult(new ErrorResponse
        {
            Message = "Validation failed.",
            StatusCode = StatusCodes.Status400BadRequest,
            Errors = errors,
            TraceId = context.HttpContext.TraceIdentifier
        });
    };
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        if (exception is AppException appException)
        {
            context.Response.StatusCode = appException.StatusCode;
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Message = appException.Message,
                StatusCode = appException.StatusCode,
                TraceId = context.TraceIdentifier
            });
            return;
        }

        if (exception is BadHttpRequestException badHttpRequestException)
        {
            context.Response.StatusCode = badHttpRequestException.StatusCode;
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Message = badHttpRequestException.Message,
                StatusCode = badHttpRequestException.StatusCode,
                TraceId = context.TraceIdentifier
            });
            return;
        }

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = "Internal server error",
            StatusCode = 500,
            TraceId = context.TraceIdentifier
        });
    });
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.EnsureCreated();
    EnsureUsersSchema(dbContext);
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

static void EnsureUsersSchema(AuthDbContext dbContext)
{
    dbContext.Database.ExecuteSqlRaw("""
        ALTER TABLE "Users"
        ADD COLUMN IF NOT EXISTS "RefreshTokenHash" text NULL;
        """);

    dbContext.Database.ExecuteSqlRaw("""
        ALTER TABLE "Users"
        ADD COLUMN IF NOT EXISTS "RefreshTokenExpiresAt" timestamp with time zone NULL;
        """);

    dbContext.Database.ExecuteSqlRaw("""
        ALTER TABLE "Users"
        ADD COLUMN IF NOT EXISTS "RefreshTokenCreatedAt" timestamp with time zone NULL;
        """);

    dbContext.Database.ExecuteSqlRaw("""
        ALTER TABLE "Users"
        ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW();
        """);
}
