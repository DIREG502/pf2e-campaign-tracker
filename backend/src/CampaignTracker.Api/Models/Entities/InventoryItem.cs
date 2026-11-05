namespace CampaignTracker.Api.Models.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;

    /// <summary>Bulk per item. Light bulk ("L") is stored as 0.1.</summary>
    public decimal Bulk { get; set; }

    /// <summary>Price per item in gold pieces.</summary>
    public decimal Price { get; set; }

    public string? Description { get; set; }
}
