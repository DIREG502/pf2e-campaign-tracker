namespace CampaignTracker.Api.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CampaignTracker";
    public string Audience { get; set; } = "CampaignTracker";

    /// <summary>Symmetric signing key. Must be at least 32 characters. Never commit it.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60 * 24;
}
