using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CampaignTracker.Api.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtOptions.SectionName);
        var options = section.Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(options.Key) || options.Key.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key must be set to at least 32 characters. " +
                "Copy appsettings.Example.json to appsettings.Development.json or set the Jwt__Key environment variable.");
        }

        services.Configure<JwtOptions>(section);
        services.AddScoped<TokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                // Keep the original claim names ("sub", "email") instead of mapping them to long URIs.
                jwt.MapInboundClaims = false;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "name"
                };
            });

        services.AddAuthorization();

        return services;
    }
}
