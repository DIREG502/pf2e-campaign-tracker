using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/campaigns")]
public class CampaignsController : ControllerBase
{
    private readonly CampaignService _campaigns;

    public CampaignsController(CampaignService campaigns)
    {
        _campaigns = campaigns;
    }

    [HttpGet]
    public async Task<ActionResult<List<CampaignDto>>> GetAll()
    {
        return await _campaigns.GetAllAsync(User.GetUserId());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CampaignDto>> Get(int id)
    {
        var campaign = await _campaigns.GetAsync(User.GetUserId(), id);
        return campaign is null ? NotFound() : campaign;
    }

    [HttpPost]
    public async Task<ActionResult<CampaignDto>> Create(CampaignRequest request)
    {
        var campaign = await _campaigns.CreateAsync(User.GetUserId(), request);
        return CreatedAtAction(nameof(Get), new { id = campaign.Id }, campaign);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CampaignRequest request)
    {
        return await _campaigns.UpdateAsync(User.GetUserId(), id, request) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _campaigns.DeleteAsync(User.GetUserId(), id) ? NoContent() : NotFound();
    }
}
