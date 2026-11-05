using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class ExpenseService
{
    private readonly AppDbContext _db;

    public ExpenseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ExpenseDto>> GetAllAsync(int userId, ExpenseFilter filter)
    {
        return await Filtered(userId, filter)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.Id)
            .Select(e => new ExpenseDto(
                e.Id, e.Amount, e.Date, e.Description, e.CategoryId, e.Category!.Name, e.Category.Color))
            .ToListAsync();
    }

    public async Task<ExpenseDto?> GetAsync(int userId, int id)
    {
        return await _db.Expenses
            .Where(e => e.UserId == userId && e.Id == id)
            .Select(e => new ExpenseDto(
                e.Id, e.Amount, e.Date, e.Description, e.CategoryId, e.Category!.Name, e.Category.Color))
            .SingleOrDefaultAsync();
    }

    public async Task<int> CreateAsync(int userId, ExpenseRequest request)
    {
        var expense = new Expense { UserId = userId };
        Apply(expense, request);

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return expense.Id;
    }

    public async Task<bool> UpdateAsync(int userId, int id, ExpenseRequest request)
    {
        var expense = await _db.Expenses.SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (expense is null)
        {
            return false;
        }

        Apply(expense, request);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var expense = await _db.Expenses.SingleOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (expense is null)
        {
            return false;
        }

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>Expenses of one user, narrowed down by the optional filter values.</summary>
    public IQueryable<Expense> Filtered(int userId, ExpenseFilter filter)
    {
        var query = _db.Expenses.Where(e => e.UserId == userId);

        if (filter.From is not null)
        {
            query = query.Where(e => e.Date >= filter.From);
        }

        if (filter.To is not null)
        {
            query = query.Where(e => e.Date <= filter.To);
        }

        if (filter.CategoryId is not null)
        {
            query = query.Where(e => e.CategoryId == filter.CategoryId);
        }

        return query;
    }

    private static void Apply(Expense expense, ExpenseRequest request)
    {
        expense.Amount = Math.Round(request.Amount, 2);
        expense.Date = request.Date!.Value;
        expense.Description = request.Description.Trim();
        expense.CategoryId = request.CategoryId!.Value;
    }
}
