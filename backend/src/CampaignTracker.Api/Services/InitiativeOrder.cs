using CampaignTracker.Api.Models.Entities;

namespace CampaignTracker.Api.Services;

/// <summary>
/// Pure turn-order logic for the initiative tracker. Kept free of EF Core so it is easy to test.
/// </summary>
public static class InitiativeOrder
{
    /// <summary>
    /// Highest initiative first. On a tie, PF2e has adversaries act before player
    /// characters, so non-PCs come first. Remaining ties fall back to name.
    /// </summary>
    public static List<Combatant> Sort(IEnumerable<Combatant> combatants)
    {
        return combatants
            .OrderByDescending(c => c.Initiative)
            .ThenBy(c => c.IsPlayerCharacter)
            .ThenBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Id)
            .ToList();
    }

    /// <summary>
    /// Advances to the next turn. Round 0 means combat has not started yet,
    /// so the first call starts round 1 with the first combatant.
    /// </summary>
    public static (int TurnIndex, int Round) Next(int combatantCount, int currentIndex, int round)
    {
        if (combatantCount == 0)
        {
            return (0, round);
        }

        if (round == 0)
        {
            return (0, 1);
        }

        var nextIndex = currentIndex + 1;
        return nextIndex >= combatantCount
            ? (0, round + 1)
            : (nextIndex, round);
    }
}
