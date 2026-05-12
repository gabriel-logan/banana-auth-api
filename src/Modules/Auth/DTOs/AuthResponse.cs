using Banana.Auth.Api.Modules.Users.DTOs;

namespace Banana.Auth.Api.Modules.Auth.DTOs;

public record AuthResponse(string Token, UserResponse User);
