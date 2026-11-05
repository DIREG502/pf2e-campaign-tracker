using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class SessionNoteService
{
    private readonly AppDbContext _db;

    public SessionNoteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SessionNoteDto>> GetAllAsync(int userId, int campaignId, string? tag = null)
    {
        var notes = await NotesOf(userId, campaignId)
            .OrderByDescending(n => n.SessionDate)
            .ThenByDescending(n => n.Id)
            .ToListAsync();

        // Tags are stored as one comma-separated string, so filter after loading.
        if (!string.IsNullOrWhiteSpace(tag))
        {
            var wanted = NormalizeTag(tag);
            notes = notes.Where(n => ParseTags(n.Tags).Contains(wanted)).ToList();
        }

        return notes.Select(ToDto).ToList();
    }

    public async Task<SessionNoteDto?> GetAsync(int userId, int campaignId, int id)
    {
        var note = await NotesOf(userId, campaignId).SingleOrDefaultAsync(n => n.Id == id);
        return note is null ? null : ToDto(note);
    }

    public async Task<SessionNoteDto> CreateAsync(int campaignId, SessionNoteRequest request)
    {
        var note = new SessionNote { CampaignId = campaignId };
        Apply(note, request);

        _db.SessionNotes.Add(note);
        await _db.SaveChangesAsync();
        return ToDto(note);
    }

    public async Task<bool> UpdateAsync(int userId, int campaignId, int id, SessionNoteRequest request)
    {
        var note = await NotesOf(userId, campaignId).SingleOrDefaultAsync(n => n.Id == id);
        if (note is null)
        {
            return false;
        }

        Apply(note, request);
        note.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int campaignId, int id)
    {
        var note = await NotesOf(userId, campaignId).SingleOrDefaultAsync(n => n.Id == id);
        if (note is null)
        {
            return false;
        }

        _db.SessionNotes.Remove(note);
        await _db.SaveChangesAsync();
        return true;
    }

    public static List<string> ParseTags(string tags)
    {
        return tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    private IQueryable<SessionNote> NotesOf(int userId, int campaignId)
    {
        return _db.SessionNotes.Where(n => n.CampaignId == campaignId && n.Campaign!.UserId == userId);
    }

    private static void Apply(SessionNote note, SessionNoteRequest request)
    {
        note.Title = request.Title.Trim();
        note.SessionDate = request.SessionDate!.Value;
        note.Content = request.Content;
        note.Tags = string.Join(',', request.Tags
            .Select(NormalizeTag)
            .Where(t => t.Length > 0)
            .Distinct());
    }

    // Commas would break the stored format, so they are removed from tag names.
    private static string NormalizeTag(string tag) => tag.Replace(",", "").Trim().ToLowerInvariant();

    private static SessionNoteDto ToDto(SessionNote note) => new(
        note.Id, note.CampaignId, note.Title, note.SessionDate, note.Content,
        ParseTags(note.Tags), note.CreatedAt, note.UpdatedAt);
}
