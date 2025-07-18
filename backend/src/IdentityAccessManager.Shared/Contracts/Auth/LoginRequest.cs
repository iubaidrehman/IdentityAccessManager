using System.ComponentModel.DataAnnotations;

namespace IdentityAccessManager.Shared.Contracts.Auth;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
} 