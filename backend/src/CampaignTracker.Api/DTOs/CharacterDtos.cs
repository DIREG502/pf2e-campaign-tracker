using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class CharacterRequest : IValidatableObject
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string Ancestry { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string Class { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Level { get; set; } = 1;

    [Range(0, 9999)]
    public int CurrentHp { get; set; }

    [Range(1, 9999)]
    public int MaxHp { get; set; }

    [Range(0, 99)]
    public int ArmorClass { get; set; }

    // Ability modifiers (PF2e Remaster), typically -5 to +7.
    [Range(-5, 10)] public int Strength { get; set; }
    [Range(-5, 10)] public int Dexterity { get; set; }
    [Range(-5, 10)] public int Constitution { get; set; }
    [Range(-5, 10)] public int Intelligence { get; set; }
    [Range(-5, 10)] public int Wisdom { get; set; }
    [Range(-5, 10)] public int Charisma { get; set; }

    [MaxLength(4000)]
    public string? Notes { get; set; }

    public int? CampaignId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CurrentHp > MaxHp)
        {
            yield return new ValidationResult(
                "Current HP cannot be higher than max HP.",
                new[] { nameof(CurrentHp) });
        }
    }
}

public record CharacterDto(
    int Id,
    string Name,
    string Ancestry,
    string Class,
    int Level,
    int CurrentHp,
    int MaxHp,
    int ArmorClass,
    int Strength,
    int Dexterity,
    int Constitution,
    int Intelligence,
    int Wisdom,
    int Charisma,
    string? Notes,
    int? CampaignId,
    string? CampaignName,
    int ItemCount);
