using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public enum CategoryResult
{
    Success,
    NotFound,
    DuplicateName,
    InUse
}

public class CategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetAllAsync(int userId)
    {
        return await _db.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Color, c.Expenses.Count))
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetAsync(int userId, int id)
    {
        return await _db.Categories
            .Where(c => c.UserId == userId && c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Color, c.Expenses.Count))
            .SingleOrDefaultAsync();
    }

    public async Task<(CategoryResult Result, CategoryDto? Category)> CreateAsync(int userId, CategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await NameTakenAsync(userId, name, exceptId: null))
        {
            return (CategoryResult.DuplicateName, null);
        }

        var category = new Category { UserId = userId, Name = name, Color = request.Color.ToLowerInvariant() };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return (CategoryResult.Success, new CategoryDto(category.Id, category.Name, category.Color, 0));
    }

    public async Task<CategoryResult> UpdateAsync(int userId, int id, CategoryRequest request)
    {
        var category = await _db.Categories.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (category is null)
        {
            return CategoryResult.NotFound;
        }

        var name = request.Name.Trim();
        if (await NameTakenAsync(userId, name, exceptId: id))
        {
            return CategoryResult.DuplicateName;
        }

        category.Name = name;
        category.Color = request.Color.ToLowerInvariant();
        await _db.SaveChangesAsync();
        return CategoryResult.Success;
    }

    public async Task<CategoryResult> DeleteAsync(int userId, int id)
    {
        var category = await _db.Categories.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == id);
        if (category is null)
        {
            return CategoryResult.NotFound;
        }

        if (await _db.Expenses.AnyAsync(e => e.CategoryId == id))
        {
            return CategoryResult.InUse;
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return CategoryResult.Success;
    }

    public Task<bool> ExistsAsync(int userId, int id)
    {
        return _db.Categories.AnyAsync(c => c.UserId == userId && c.Id == id);
    }

    private Task<bool> NameTakenAsync(int userId, string name, int? exceptId)
    {
        var lower = name.ToLower();
        return _db.Categories.AnyAsync(c =>
            c.UserId == userId && c.Name.ToLower() == lower && c.Id != exceptId);
    }
}
