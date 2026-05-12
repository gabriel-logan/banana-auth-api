namespace Banana.Auth.Api.Modules.Users.DTOs;

public record UserResponse(Guid Id, string Email, DateTime CreatedAt, DateTime UpdatedAt);
