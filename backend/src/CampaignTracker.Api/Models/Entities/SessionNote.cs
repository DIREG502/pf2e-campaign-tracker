namespace CampaignTracker.Api.Models.Entities;

public class SessionNote
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }

    /// <summary>Note body. Plain text or Markdown.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Comma-separated list of tags, e.g. "combat,loot".</summary>
    public string Tags { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
