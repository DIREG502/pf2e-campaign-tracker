using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

/// <summary>Initiative tracker. Mutating combat endpoints return the full, sorted encounter.</summary>
[ApiController]
[Authorize]
[Route("api/encounters")]
public class EncountersController : ControllerBase
{
    private readonly EncounterService _encounters;
    private readonly CampaignService _campaigns;

    public EncountersController(EncounterService encounters, CampaignService campaigns)
    {
        _encounters = encounters;
        _campaigns = campaigns;
    }

    [HttpGet]
    public async Task<ActionResult<List<EncounterSummaryDto>>> GetAll()
    {
        return await _encounters.GetAllAsync(User.GetUserId());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EncounterDto>> Get(int id)
    {
        return OkOrNotFound(await _encounters.GetAsync(User.GetUserId(), id));
    }

    [HttpPost]
    public async Task<ActionResult<EncounterDto>> Create(EncounterRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCampaignAsync(userId, request.CampaignId))
        {
            return ValidationProblem(ModelState);
        }

        var encounter = await _encounters.CreateAsync(userId, request);
        return CreatedAtAction(nameof(Get), new { id = encounter.Id }, encounter);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EncounterRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCampaignAsync(userId, request.CampaignId))
        {
            return ValidationProblem(ModelState);
        }

        return await _encounters.UpdateAsync(userId, id, request) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _encounters.DeleteAsync(User.GetUserId(), id) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/combatants")]
    public async Task<ActionResult<EncounterDto>> AddCombatant(int id, CombatantRequest request)
    {
        return OkOrNotFound(await _encounters.AddCombatantAsync(User.GetUserId(), id, request));
    }

    [HttpPost("{id:int}/combatants/from-character")]
    public async Task<ActionResult<EncounterDto>> AddCharacter(int id, AddCharacterCombatantRequest request)
    {
        return OkOrNotFound(await _encounters.AddCharacterAsync(User.GetUserId(), id, request));
    }

    [HttpPut("{id:int}/combatants/{combatantId:int}")]
    public async Task<ActionResult<EncounterDto>> UpdateCombatant(int id, int combatantId, CombatantRequest request)
    {
        return OkOrNotFound(await _encounters.UpdateCombatantAsync(User.GetUserId(), id, combatantId, request));
    }

    [HttpDelete("{id:int}/combatants/{combatantId:int}")]
    public async Task<ActionResult<EncounterDto>> RemoveCombatant(int id, int combatantId)
    {
        return OkOrNotFound(await _encounters.RemoveCombatantAsync(User.GetUserId(), id, combatantId));
    }

    [HttpPost("{id:int}/next-turn")]
    public async Task<ActionResult<EncounterDto>> NextTurn(int id)
    {
        return OkOrNotFound(await _encounters.NextTurnAsync(User.GetUserId(), id));
    }

    [HttpPost("{id:int}/reset")]
    public async Task<ActionResult<EncounterDto>> Reset(int id)
    {
        return OkOrNotFound(await _encounters.ResetAsync(User.GetUserId(), id));
    }

    private ActionResult<EncounterDto> OkOrNotFound(EncounterDto? encounter)
    {
        return encounter is null ? NotFound() : Ok(encounter);
    }

    private async Task<bool> IsValidCampaignAsync(int userId, int? campaignId)
    {
        if (campaignId is null || await _campaigns.ExistsAsync(userId, campaignId.Value))
        {
            return true;
        }

        ModelState.AddModelError(nameof(EncounterRequest.CampaignId), "Campaign not found.");
        return false;
    }
}
