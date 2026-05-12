using System.ComponentModel.DataAnnotations;

namespace Banana.Auth.Api.Modules.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}
