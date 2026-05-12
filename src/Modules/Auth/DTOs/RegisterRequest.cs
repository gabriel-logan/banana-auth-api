using System.ComponentModel.DataAnnotations;

namespace Banana.Auth.Api.Modules.Auth.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}
