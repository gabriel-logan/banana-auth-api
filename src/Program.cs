using Banana.Auth.Api.Modules.Auth;
using Banana.Auth.Api.Modules.Users;
using Banana.Auth.Api.Infrastructure.Database;
using Banana.Auth.Api.Infrastructure.Security;
using Banana.Auth.Api.Common.Exceptions;
using Banana.Auth.Api.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowedDomains = builder.Configuration["ALLOWED_DOMAINS"] ?? "";
var normalizedAllowedDomains = ExpandAllowedDomains(allowedDomains);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        if (allowedDomains.Trim() == "*")
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        if (normalizedAllowedDomains.Length > 0)
        {
            policy
                .WithOrigins(normalizedAllowedDomains)
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
builder.Services.AddScoped<AuthDbInitializer>();
builder.Services.AddScoped<AuthDbSeeder>();

var app = builder.Build();

app.UseCors("FrontendCors");
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
    var initializer = scope.ServiceProvider.GetRequiredService<AuthDbInitializer>();
    await initializer.InitializeAsync();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

static string[] ExpandAllowedDomains(string allowedDomains)
{
    var origins = allowedDomains
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var expandedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (var origin in origins)
    {
        expandedOrigins.Add(origin);

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
        {
            continue;
        }

        if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("::1", StringComparison.OrdinalIgnoreCase))
        {
            var port = uri.IsDefaultPort ? string.Empty : $":{uri.Port}";
            expandedOrigins.Add($"{uri.Scheme}://localhost{port}");
            expandedOrigins.Add($"{uri.Scheme}://127.0.0.1{port}");
            expandedOrigins.Add($"{uri.Scheme}://[::1]{port}");
        }
    }

    return expandedOrigins.ToArray();
}
