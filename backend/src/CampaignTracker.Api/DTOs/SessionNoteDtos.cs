using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class SessionNoteRequest
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateOnly? SessionDate { get; set; }

    [MaxLength(20000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(20)]
    public List<string> Tags { get; set; } = new();
}

public record SessionNoteDto(
    int Id,
    int CampaignId,
    string Title,
    DateOnly SessionDate,
    string Content,
    List<string> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt);
