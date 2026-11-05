using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record UserDto(int Id, string Email, string DisplayName);

public record AuthResponse(string Token, DateTime ExpiresAt, UserDto User);
