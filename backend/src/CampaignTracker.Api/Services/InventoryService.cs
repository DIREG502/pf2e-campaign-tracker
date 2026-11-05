using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

/// <summary>
/// Inventory items of a character. All methods check that the character
/// belongs to the given user, so a user can never touch someone else's items.
/// </summary>
public class InventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<InventoryItemDto>> GetAllAsync(int userId, int characterId)
    {
        var items = await ItemsOf(userId, characterId)
            .OrderBy(i => i.Name)
            .ToListAsync();

        return items.Select(ToDto).ToList();
    }

    public async Task<InventoryItemDto?> GetAsync(int userId, int characterId, int id)
    {
        var item = await ItemsOf(userId, characterId).SingleOrDefaultAsync(i => i.Id == id);
        return item is null ? null : ToDto(item);
    }

    public async Task<InventoryItemDto> CreateAsync(int characterId, InventoryItemRequest request)
    {
        var item = new InventoryItem { CharacterId = characterId };
        Apply(item, request);

        _db.InventoryItems.Add(item);
        await _db.SaveChangesAsync();
        return ToDto(item);
    }

    public async Task<bool> UpdateAsync(int userId, int characterId, int id, InventoryItemRequest request)
    {
        var item = await ItemsOf(userId, characterId).SingleOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return false;
        }

        Apply(item, request);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int characterId, int id)
    {
        var item = await ItemsOf(userId, characterId).SingleOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return false;
        }

        _db.InventoryItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    private IQueryable<InventoryItem> ItemsOf(int userId, int characterId)
    {
        return _db.InventoryItems.Where(i => i.CharacterId == characterId && i.Character!.UserId == userId);
    }

    private static void Apply(InventoryItem item, InventoryItemRequest request)
    {
        item.Name = request.Name.Trim();
        item.Quantity = request.Quantity;
        item.Bulk = request.Bulk;
        item.Price = request.Price;
        item.Description = request.Description;
    }

    private static InventoryItemDto ToDto(InventoryItem item) => new(
        item.Id, item.CharacterId, item.Name, item.Quantity, item.Bulk, item.Price, item.Description);
}
