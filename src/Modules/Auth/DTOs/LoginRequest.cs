using System.ComponentModel.DataAnnotations;

namespace Banana.Auth.Api.Modules.Auth.DTOs;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
