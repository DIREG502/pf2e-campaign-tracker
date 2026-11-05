namespace CampaignTracker.Api.Models.Entities;

public class Combatant
{
    public int Id { get; set; }
    public int EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    /// <summary>Set when the combatant was added from an existing character.</summary>
    public int? CharacterId { get; set; }
    public Character? Character { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Initiative { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }

    /// <summary>Comma-separated conditions, e.g. "frightened 1,off-guard".</summary>
    public string Conditions { get; set; } = string.Empty;

    public bool IsPlayerCharacter { get; set; }
}
