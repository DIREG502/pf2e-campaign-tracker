using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class CategoryRequest
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Color must be a hex value like #2a78d6.")]
    public string Color { get; set; } = "#888888";
}

public record CategoryDto(int Id, string Name, string Color, int ExpenseCount);
