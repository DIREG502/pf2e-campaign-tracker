using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class CampaignService
{
    private readonly AppDbContext _db;

    public CampaignService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CampaignDto>> GetAllAsync(int userId)
    {
        return await _db.Campaigns
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CampaignDto(c.Id, c.Name, c.Description, c.CreatedAt, c.SessionNotes.Count))
            .ToListAsync();
    }

    public async Task<CampaignDto?> GetAsync(int userId, int id)
    {
        return await _db.Campaigns
            .Where(c => c.UserId == userId && c.Id == id)
            .Select(c => new CampaignDto(c.Id, c.Name, c.Description, c.CreatedAt, c.SessionNotes.Count))
            .SingleOrDefaultAsync();
    }

    public async Task<CampaignDto> CreateAsync(int userId, CampaignRequest request)
    {
        var campaign = new Campaign
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Description = request.Description
        };

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();

        return new CampaignDto(campaign.Id, campaign.Name, campaign.Description, campaign.CreatedAt, 0);
    }

    public async Task<bool> UpdateAsync(int userId, int id, CampaignRequest request)
    {
        var campaign = await _db.Campaigns.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (campaign is null)
        {
            return false;
        }

        campaign.Name = request.Name.Trim();
        campaign.Description = request.Description;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var campaign = await _db.Campaigns.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (campaign is null)
        {
            return false;
        }

        _db.Campaigns.Remove(campaign);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int userId, int id)
    {
        return _db.Campaigns.AnyAsync(c => c.UserId == userId && c.Id == id);
    }
}
