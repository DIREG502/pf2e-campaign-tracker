using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class InventoryItemRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 9999)]
    public int Quantity { get; set; } = 1;

    /// <summary>Bulk per item. Use 0.1 for light bulk ("L").</summary>
    [Range(0, 100)]
    public decimal Bulk { get; set; }

    /// <summary>Price per item in gold pieces.</summary>
    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public record InventoryItemDto(
    int Id,
    int CharacterId,
    string Name,
    int Quantity,
    decimal Bulk,
    decimal Price,
    string? Description);
