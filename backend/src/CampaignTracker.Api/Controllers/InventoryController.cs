using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/characters/{characterId:int}/items")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventory;
    private readonly CharacterService _characters;

    public InventoryController(InventoryService inventory, CharacterService characters)
    {
        _inventory = inventory;
        _characters = characters;
    }

    [HttpGet]
    public async Task<ActionResult<List<InventoryItemDto>>> GetAll(int characterId)
    {
        var userId = User.GetUserId();
        if (!await _characters.ExistsAsync(userId, characterId))
        {
            return NotFound();
        }

        return await _inventory.GetAllAsync(userId, characterId);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryItemDto>> Get(int characterId, int id)
    {
        var item = await _inventory.GetAsync(User.GetUserId(), characterId, id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItemDto>> Create(int characterId, InventoryItemRequest request)
    {
        if (!await _characters.ExistsAsync(User.GetUserId(), characterId))
        {
            return NotFound();
        }

        var item = await _inventory.CreateAsync(characterId, request);
        return CreatedAtAction(nameof(Get), new { characterId, id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int characterId, int id, InventoryItemRequest request)
    {
        var updated = await _inventory.UpdateAsync(User.GetUserId(), characterId, id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int characterId, int id)
    {
        var deleted = await _inventory.DeleteAsync(User.GetUserId(), characterId, id);
        return deleted ? NoContent() : NotFound();
    }
}
