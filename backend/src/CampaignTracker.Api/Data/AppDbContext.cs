using CampaignTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CampaignTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<SessionNote> SessionNotes => Set<SessionNote>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Combatant> Combatants => Set<Combatant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // All timestamps are written as UTC. SQLite does not store the kind, so mark
        // values as UTC when reading them back; otherwise the JSON loses its "Z" suffix.
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    private class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Email).HasMaxLength(256);
            user.Property(u => u.DisplayName).HasMaxLength(100);
        });

        modelBuilder.Entity<Campaign>(campaign =>
        {
            campaign.Property(c => c.Name).HasMaxLength(100);
            campaign.Property(c => c.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Character>(character =>
        {
            character.Property(c => c.Name).HasMaxLength(100);
            character.Property(c => c.Ancestry).HasMaxLength(50);
            character.Property(c => c.Class).HasMaxLength(50);
            character.Property(c => c.Notes).HasMaxLength(4000);

            // Deleting a campaign keeps its characters, they just become unassigned.
            character.HasOne(c => c.Campaign)
                .WithMany()
                .HasForeignKey(c => c.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<InventoryItem>(item =>
        {
            item.Property(i => i.Name).HasMaxLength(100);
            item.Property(i => i.Description).HasMaxLength(1000);
            item.Property(i => i.Bulk).HasPrecision(6, 1);
            item.Property(i => i.Price).HasPrecision(12, 2);
        });

        modelBuilder.Entity<SessionNote>(note =>
        {
            note.Property(n => n.Title).HasMaxLength(200);
            note.Property(n => n.Tags).HasMaxLength(500);
        });

        modelBuilder.Entity<Encounter>(encounter =>
        {
            encounter.Property(e => e.Name).HasMaxLength(100);
            encounter.HasOne(e => e.Campaign)
                .WithMany()
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Combatant>(combatant =>
        {
            combatant.Property(c => c.Name).HasMaxLength(100);
            combatant.Property(c => c.Conditions).HasMaxLength(500);
            combatant.HasOne(c => c.Character)
                .WithMany()
                .HasForeignKey(c => c.CharacterId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(50);
            category.Property(c => c.Color).HasMaxLength(7);
            category.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
        });

        modelBuilder.Entity<Expense>(expense =>
        {
            expense.Property(e => e.Amount).HasPrecision(12, 2);
            expense.Property(e => e.Description).HasMaxLength(200);
            expense.HasIndex(e => new { e.UserId, e.Date });

            // A category that still has expenses cannot be deleted.
            expense.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
