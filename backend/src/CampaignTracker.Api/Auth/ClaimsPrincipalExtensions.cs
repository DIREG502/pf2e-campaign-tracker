using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CampaignTracker.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Reads the user id from the "sub" claim of the JWT.</summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(value, out var userId))
        {
            throw new InvalidOperationException("The current user has no valid 'sub' claim.");
        }

        return userId;
    }
}
