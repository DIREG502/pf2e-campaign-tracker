namespace CampaignTracker.Api.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Hex color used in charts, e.g. "#4e79a7".</summary>
    public string Color { get; set; } = "#888888";

    public List<Expense> Expenses { get; set; } = new();
}
