using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class EncounterService
{
    private readonly AppDbContext _db;

    public EncounterService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EncounterSummaryDto>> GetAllAsync(int userId)
    {
        return await _db.Encounters
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EncounterSummaryDto(e.Id, e.Name, e.CampaignId, e.Round, e.Combatants.Count, e.CreatedAt))
            .ToListAsync();
    }

    public async Task<EncounterDto?> GetAsync(int userId, int id)
    {
        var encounter = await LoadAsync(userId, id);
        return encounter is null ? null : ToDto(encounter);
    }

    public async Task<EncounterDto> CreateAsync(int userId, EncounterRequest request)
    {
        var encounter = new Encounter
        {
            UserId = userId,
            Name = request.Name.Trim(),
            CampaignId = request.CampaignId
        };

        _db.Encounters.Add(encounter);
        await _db.SaveChangesAsync();
        return ToDto(encounter);
    }

    public async Task<bool> UpdateAsync(int userId, int id, EncounterRequest request)
    {
        var encounter = await _db.Encounters.SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (encounter is null)
        {
            return false;
        }

        encounter.Name = request.Name.Trim();
        encounter.CampaignId = request.CampaignId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var encounter = await _db.Encounters.SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (encounter is null)
        {
            return false;
        }

        _db.Encounters.Remove(encounter);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<EncounterDto?> AddCombatantAsync(int userId, int encounterId, CombatantRequest request)
    {
        var encounter = await LoadAsync(userId, encounterId);
        if (encounter is null)
        {
            return null;
        }

        var combatant = new Combatant();
        Apply(combatant, request);

        await ChangeCombatantsAsync(encounter, () => encounter.Combatants.Add(combatant));
        return ToDto(encounter);
    }

    /// <summary>Adds an existing character, copying its name and HP. Returns null if either is missing.</summary>
    public async Task<EncounterDto?> AddCharacterAsync(int userId, int encounterId, AddCharacterCombatantRequest request)
    {
        var encounter = await LoadAsync(userId, encounterId);
        var character = await _db.Characters
            .SingleOrDefaultAsync(c => c.UserId == userId && c.Id == request.CharacterId);

        if (encounter is null || character is null)
        {
            return null;
        }

        var combatant = new Combatant
        {
            CharacterId = character.Id,
            Name = character.Name,
            Initiative = request.Initiative,
            CurrentHp = character.CurrentHp,
            MaxHp = character.MaxHp,
            IsPlayerCharacter = true
        };

        await ChangeCombatantsAsync(encounter, () => encounter.Combatants.Add(combatant));
        return ToDto(encounter);
    }

    public async Task<EncounterDto?> UpdateCombatantAsync(int userId, int encounterId, int combatantId, CombatantRequest request)
    {
        var encounter = await LoadAsync(userId, encounterId);
        var combatant = encounter?.Combatants.SingleOrDefault(c => c.Id == combatantId);

        if (encounter is null || combatant is null)
        {
            return null;
        }

        // Changing initiative can reorder the list, so keep the turn on the same combatant.
        await ChangeCombatantsAsync(encounter, () => Apply(combatant, request));
        return ToDto(encounter);
    }

    public async Task<EncounterDto?> RemoveCombatantAsync(int userId, int encounterId, int combatantId)
    {
        var encounter = await LoadAsync(userId, encounterId);
        var combatant = encounter?.Combatants.SingleOrDefault(c => c.Id == combatantId);

        if (encounter is null || combatant is null)
        {
            return null;
        }

        await ChangeCombatantsAsync(encounter, () => encounter.Combatants.Remove(combatant));
        return ToDto(encounter);
    }

    public async Task<EncounterDto?> NextTurnAsync(int userId, int encounterId)
    {
        var encounter = await LoadAsync(userId, encounterId);
        if (encounter is null)
        {
            return null;
        }

        var (turnIndex, round) = InitiativeOrder.Next(encounter.Combatants.Count, encounter.CurrentTurnIndex, encounter.Round);
        encounter.CurrentTurnIndex = turnIndex;
        encounter.Round = round;

        await _db.SaveChangesAsync();
        return ToDto(encounter);
    }

    public async Task<EncounterDto?> ResetAsync(int userId, int encounterId)
    {
        var encounter = await LoadAsync(userId, encounterId);
        if (encounter is null)
        {
            return null;
        }

        encounter.Round = 0;
        encounter.CurrentTurnIndex = 0;
        await _db.SaveChangesAsync();
        return ToDto(encounter);
    }

    private Task<Encounter?> LoadAsync(int userId, int id)
    {
        return _db.Encounters
            .Include(e => e.Combatants)
            .SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
    }

    /// <summary>
    /// Runs a change to the combatant list and then moves the turn pointer so it still points
    /// at whoever had the turn before. If that combatant was removed, the next one takes over.
    /// </summary>
    private async Task ChangeCombatantsAsync(Encounter encounter, Action change)
    {
        var before = InitiativeOrder.Sort(encounter.Combatants);
        var current = before.ElementAtOrDefault(encounter.CurrentTurnIndex);

        change();
        await _db.SaveChangesAsync();

        var after = InitiativeOrder.Sort(encounter.Combatants);
        if (after.Count == 0)
        {
            encounter.CurrentTurnIndex = 0;
        }
        else if (current is not null && after.Contains(current))
        {
            encounter.CurrentTurnIndex = after.IndexOf(current);
        }
        else
        {
            encounter.CurrentTurnIndex = Math.Min(encounter.CurrentTurnIndex, after.Count - 1);
        }

        await _db.SaveChangesAsync();
    }

    private static void Apply(Combatant combatant, CombatantRequest request)
    {
        combatant.Name = request.Name.Trim();
        combatant.Initiative = request.Initiative;
        combatant.CurrentHp = request.CurrentHp;
        combatant.MaxHp = request.MaxHp;
        combatant.IsPlayerCharacter = request.IsPlayerCharacter;
        combatant.Conditions = string.Join(',', request.Conditions
            .Select(c => c.Replace(",", "").Trim())
            .Where(c => c.Length > 0));
    }

    private static EncounterDto ToDto(Encounter encounter)
    {
        var ordered = InitiativeOrder.Sort(encounter.Combatants);
        var current = encounter.Round > 0 ? ordered.ElementAtOrDefault(encounter.CurrentTurnIndex) : null;

        return new EncounterDto(
            encounter.Id,
            encounter.Name,
            encounter.CampaignId,
            encounter.Round,
            encounter.CurrentTurnIndex,
            current?.Id,
            ordered.Select(c => new CombatantDto(
                c.Id,
                c.CharacterId,
                c.Name,
                c.Initiative,
                c.CurrentHp,
                c.MaxHp,
                c.Conditions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                c.IsPlayerCharacter)).ToList());
    }
}
