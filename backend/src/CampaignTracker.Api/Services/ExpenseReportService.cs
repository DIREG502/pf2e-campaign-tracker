using System.Globalization;
using CampaignTracker.Api.DTOs;

namespace CampaignTracker.Api.Services;

/// <summary>
/// Aggregations for the expense charts. Works on already loaded expenses because
/// SQLite cannot sum decimal columns in SQL, and personal expense lists are small.
/// </summary>
public static class ExpenseReportService
{
    public static ExpenseSummaryDto Summarize(IReadOnlyCollection<ExpenseDto> expenses)
    {
        var byCategory = expenses
            .GroupBy(e => new { e.CategoryId, e.CategoryName, e.CategoryColor })
            .Select(g => new CategoryTotalDto(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Key.CategoryColor,
                g.Sum(e => e.Amount),
                g.Count()))
            .OrderByDescending(c => c.Total)
            .ThenBy(c => c.Name)
            .ToList();

        return new ExpenseSummaryDto(
            expenses.Sum(e => e.Amount),
            expenses.Count,
            byCategory,
            TotalsByMonth(expenses));
    }

    /// <summary>
    /// Monthly totals from the first to the last month with expenses.
    /// Months without expenses are included with 0 so the chart has no gaps.
    /// </summary>
    public static List<MonthTotalDto> TotalsByMonth(IReadOnlyCollection<ExpenseDto> expenses)
    {
        if (expenses.Count == 0)
        {
            return new List<MonthTotalDto>();
        }

        var totals = expenses
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, 1))
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        var first = totals.Keys.Min();
        var last = totals.Keys.Max();
        var result = new List<MonthTotalDto>();

        for (var month = first; month <= last; month = month.AddMonths(1))
        {
            result.Add(new MonthTotalDto(
                month.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                totals.GetValueOrDefault(month)));
        }

        return result;
    }
}
