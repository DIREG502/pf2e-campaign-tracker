using CampaignTracker.Api.Models.Entities;

namespace CampaignTracker.Api.Data;

/// <summary>
/// Categories every new user starts with. They can be renamed or deleted later.
/// Colors are picked so neighbouring slices stay distinguishable with color vision deficiencies.
/// </summary>
public static class DefaultCategories
{
    private static readonly (string Name, string Color)[] Defaults =
    {
        ("Food", "#2a78d6"),
        ("Rent", "#eb6834"),
        ("Transport", "#1baf7a"),
        ("Entertainment", "#eda100"),
        ("Games & Hobbies", "#e87ba4"),
        ("Other", "#008300")
    };

    public static IEnumerable<Category> For(User user)
    {
        return Defaults.Select(d => new Category { User = user, Name = d.Name, Color = d.Color });
    }
}
