namespace CampaignTracker.Api.Models.Entities;

public class Encounter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Current round. 0 means combat has not started yet.</summary>
    public int Round { get; set; }

    /// <summary>Index into the sorted combatant list of whose turn it is.</summary>
    public int CurrentTurnIndex { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Combatant> Combatants { get; set; } = new();
}
