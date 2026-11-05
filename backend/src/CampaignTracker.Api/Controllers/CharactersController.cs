using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/characters")]
public class CharactersController : ControllerBase
{
    private readonly CharacterService _characters;
    private readonly CampaignService _campaigns;

    public CharactersController(CharacterService characters, CampaignService campaigns)
    {
        _characters = characters;
        _campaigns = campaigns;
    }

    [HttpGet]
    public async Task<ActionResult<List<CharacterDto>>> GetAll([FromQuery] int? campaignId)
    {
        return await _characters.GetAllAsync(User.GetUserId(), campaignId);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CharacterDto>> Get(int id)
    {
        var character = await _characters.GetAsync(User.GetUserId(), id);
        return character is null ? NotFound() : character;
    }

    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create(CharacterRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCampaignAsync(userId, request.CampaignId))
        {
            return ValidationProblem(ModelState);
        }

        var id = await _characters.CreateAsync(userId, request);
        var character = await _characters.GetAsync(userId, id);
        return CreatedAtAction(nameof(Get), new { id }, character);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CharacterRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCampaignAsync(userId, request.CampaignId))
        {
            return ValidationProblem(ModelState);
        }

        return await _characters.UpdateAsync(userId, id, request) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _characters.DeleteAsync(User.GetUserId(), id) ? NoContent() : NotFound();
    }

    private async Task<bool> IsValidCampaignAsync(int userId, int? campaignId)
    {
        if (campaignId is null || await _campaigns.ExistsAsync(userId, campaignId.Value))
        {
            return true;
        }

        ModelState.AddModelError(nameof(CharacterRequest.CampaignId), "Campaign not found.");
        return false;
    }
}
