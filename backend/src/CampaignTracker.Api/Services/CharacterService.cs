using System.Linq.Expressions;
using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class CharacterService
{
    private static readonly Expression<Func<Character, CharacterDto>> ToDto = c => new CharacterDto(
        c.Id, c.Name, c.Ancestry, c.Class, c.Level,
        c.CurrentHp, c.MaxHp, c.ArmorClass,
        c.Strength, c.Dexterity, c.Constitution, c.Intelligence, c.Wisdom, c.Charisma,
        c.Notes, c.CampaignId, c.Campaign != null ? c.Campaign.Name : null,
        c.Inventory.Count);

    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CharacterDto>> GetAllAsync(int userId, int? campaignId = null)
    {
        var query = _db.Characters.Where(c => c.UserId == userId);

        if (campaignId is not null)
        {
            query = query.Where(c => c.CampaignId == campaignId);
        }

        return await query.OrderBy(c => c.Name).Select(ToDto).ToListAsync();
    }

    public async Task<CharacterDto?> GetAsync(int userId, int id)
    {
        return await _db.Characters
            .Where(c => c.UserId == userId && c.Id == id)
            .Select(ToDto)
            .SingleOrDefaultAsync();
    }

    public async Task<int> CreateAsync(int userId, CharacterRequest request)
    {
        var character = new Character { UserId = userId };
        Apply(character, request);

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();
        return character.Id;
    }

    public async Task<bool> UpdateAsync(int userId, int id, CharacterRequest request)
    {
        var character = await _db.Characters.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (character is null)
        {
            return false;
        }

        Apply(character, request);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var character = await _db.Characters.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (character is null)
        {
            return false;
        }

        _db.Characters.Remove(character);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int userId, int id)
    {
        return _db.Characters.AnyAsync(c => c.UserId == userId && c.Id == id);
    }

    private static void Apply(Character character, CharacterRequest request)
    {
        character.Name = request.Name.Trim();
        character.Ancestry = request.Ancestry.Trim();
        character.Class = request.Class.Trim();
        character.Level = request.Level;
        character.CurrentHp = request.CurrentHp;
        character.MaxHp = request.MaxHp;
        character.ArmorClass = request.ArmorClass;
        character.Strength = request.Strength;
        character.Dexterity = request.Dexterity;
        character.Constitution = request.Constitution;
        character.Intelligence = request.Intelligence;
        character.Wisdom = request.Wisdom;
        character.Charisma = request.Charisma;
        character.Notes = request.Notes;
        character.CampaignId = request.CampaignId;
    }
}
