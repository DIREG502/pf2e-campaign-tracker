using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class CampaignRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }
}

public record CampaignDto(int Id, string Name, string? Description, DateTime CreatedAt, int SessionNoteCount);
