using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class EncounterRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public int? CampaignId { get; set; }
}

public class CombatantRequest : IValidatableObject
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(-10, 99)]
    public int Initiative { get; set; }

    [Range(0, 9999)]
    public int CurrentHp { get; set; }

    [Range(1, 9999)]
    public int MaxHp { get; set; }

    [MaxLength(20)]
    public List<string> Conditions { get; set; } = new();

    public bool IsPlayerCharacter { get; set; }

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

public class AddCharacterCombatantRequest
{
    [Required]
    public int? CharacterId { get; set; }

    [Range(-10, 99)]
    public int Initiative { get; set; }
}

public record CombatantDto(
    int Id,
    int? CharacterId,
    string Name,
    int Initiative,
    int CurrentHp,
    int MaxHp,
    List<string> Conditions,
    bool IsPlayerCharacter);

public record EncounterSummaryDto(int Id, string Name, int? CampaignId, int Round, int CombatantCount, DateTime CreatedAt);

/// <summary>Full encounter state. Combatants are already in turn order.</summary>
public record EncounterDto(
    int Id,
    string Name,
    int? CampaignId,
    int Round,
    int CurrentTurnIndex,
    int? CurrentCombatantId,
    List<CombatantDto> Combatants);
