namespace CampaignTracker.Api.DTOs;

public record CategoryTotalDto(int CategoryId, string Name, string Color, decimal Total, int Count);

/// <summary>Total for one calendar month. Month is formatted as "yyyy-MM".</summary>
public record MonthTotalDto(string Month, decimal Total);

public record ExpenseSummaryDto(
    decimal Total,
    int Count,
    List<CategoryTotalDto> ByCategory,
    List<MonthTotalDto> ByMonth);
