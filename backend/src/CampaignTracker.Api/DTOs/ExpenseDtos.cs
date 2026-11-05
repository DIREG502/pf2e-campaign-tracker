using System.ComponentModel.DataAnnotations;

namespace CampaignTracker.Api.DTOs;

public class ExpenseRequest
{
    // The limits are strings, so parse them invariantly; otherwise "0.01" fails on e.g. a German server.
    [Range(typeof(decimal), "0.01", "1000000",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "Amount must be between 0.01 and 1,000,000.")]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly? Date { get; set; }

    [Required, StringLength(200, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int? CategoryId { get; set; }
}

/// <summary>Query string filter shared by the list, summary and CSV export endpoints.</summary>
public class ExpenseFilter : IValidatableObject
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public int? CategoryId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From is not null && To is not null && From > To)
        {
            yield return new ValidationResult("'From' must be on or before 'To'.", new[] { nameof(From) });
        }
    }
}

public record ExpenseDto(
    int Id,
    decimal Amount,
    DateOnly Date,
    string Description,
    int CategoryId,
    string CategoryName,
    string CategoryColor);
