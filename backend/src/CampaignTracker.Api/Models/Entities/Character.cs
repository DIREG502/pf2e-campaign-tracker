namespace CampaignTracker.Api.Models.Entities;

/// <summary>
/// A Pathfinder 2e character. Ability values are stored as modifiers
/// (e.g. +4 Str), following the PF2e Remaster rules.
/// </summary>
public class Character
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Ancestry { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; } = 1;

    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int ArmorClass { get; set; }

    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public string? Notes { get; set; }

    public List<InventoryItem> Inventory { get; set; } = new();
}
