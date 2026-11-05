using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Data;

public static class DatabaseExtensions
{
    /// <summary>
    /// Registers <see cref="AppDbContext"/> using the provider from "Database:Provider"
    /// ("Sqlite" or "PostgreSQL") and the "Default" connection string.
    /// </summary>
    public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
        {
            switch (provider.ToLowerInvariant())
            {
                case "sqlite":
                    options.UseSqlite(connectionString);
                    break;
                case "postgresql":
                case "postgres":
                    options.UseNpgsql(connectionString);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unknown database provider '{provider}'. Use 'Sqlite' or 'PostgreSQL'.");
            }
        });

        return services;
    }

    /// <summary>Applies pending migrations on startup so the app works with zero setup.</summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}
