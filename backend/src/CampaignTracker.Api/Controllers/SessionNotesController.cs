using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/campaigns/{campaignId:int}/notes")]
public class SessionNotesController : ControllerBase
{
    private readonly SessionNoteService _notes;
    private readonly CampaignService _campaigns;

    public SessionNotesController(SessionNoteService notes, CampaignService campaigns)
    {
        _notes = notes;
        _campaigns = campaigns;
    }

    [HttpGet]
    public async Task<ActionResult<List<SessionNoteDto>>> GetAll(int campaignId, [FromQuery] string? tag)
    {
        var userId = User.GetUserId();
        if (!await _campaigns.ExistsAsync(userId, campaignId))
        {
            return NotFound();
        }

        return await _notes.GetAllAsync(userId, campaignId, tag);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SessionNoteDto>> Get(int campaignId, int id)
    {
        var note = await _notes.GetAsync(User.GetUserId(), campaignId, id);
        return note is null ? NotFound() : note;
    }

    [HttpPost]
    public async Task<ActionResult<SessionNoteDto>> Create(int campaignId, SessionNoteRequest request)
    {
        if (!await _campaigns.ExistsAsync(User.GetUserId(), campaignId))
        {
            return NotFound();
        }

        var note = await _notes.CreateAsync(campaignId, request);
        return CreatedAtAction(nameof(Get), new { campaignId, id = note.Id }, note);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int campaignId, int id, SessionNoteRequest request)
    {
        var updated = await _notes.UpdateAsync(User.GetUserId(), campaignId, id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int campaignId, int id)
    {
        var deleted = await _notes.DeleteAsync(User.GetUserId(), campaignId, id);
        return deleted ? NoContent() : NotFound();
    }
}
