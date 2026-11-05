using CampaignTracker.Api.Models.Entities;

namespace CampaignTracker.Api.Data;

/// <summary>Categories every new user starts with. They can be renamed or deleted later.</summary>
public static class DefaultCategories
{
    private static readonly (string Name, string Color)[] Defaults =
    {
        ("Food", "#e15759"),
        ("Rent", "#4e79a7"),
        ("Transport", "#f28e2b"),
        ("Entertainment", "#76b7b2"),
        ("Games & Hobbies", "#59a14f"),
        ("Other", "#bab0ac")
    };

    public static IEnumerable<Category> For(User user)
    {
        return Defaults.Select(d => new Category { User = user, Name = d.Name, Color = d.Color });
    }
}
