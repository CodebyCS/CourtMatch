using Game.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Game.Infrastructure.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Game> Games => Set<Domain.Entities.Game>();
    public DbSet<Domain.Entities.GameParticipant> GameParticipants => Set<Domain.Entities.GameParticipant>();
    public DbSet<Domain.Entities.GameSet> GameSets => Set<Domain.Entities.GameSet>();
    public DbSet<Domain.Entities.PlayerRanking> PlayerRankings => Set<Domain.Entities.PlayerRanking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Game>(entity =>
        {
            entity.ToTable("games");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(g => g.Format).HasConversion<string>().HasMaxLength(20);
            entity.Property(g => g.Name).HasMaxLength(100);
            entity.HasIndex(g => g.BookingId); // not unique: a booking can have more than one game

            entity.HasMany(g => g.Participants)
                .WithOne()
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Sets)
                .WithOne()
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata.FindNavigation(nameof(Domain.Entities.Game.Participants))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            entity.Metadata.FindNavigation(nameof(Domain.Entities.Game.Sets))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Domain.Entities.GameParticipant>(entity =>
        {
            entity.ToTable("game_participants");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(p => new { p.GameId, p.UserId }).IsUnique();
        });

        modelBuilder.Entity<Domain.Entities.GameSet>(entity =>
        {
            entity.ToTable("game_sets");
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => new { s.GameId, s.SetNumber }).IsUnique();
        });

        modelBuilder.Entity<Domain.Entities.PlayerRanking>(entity =>
        {
            entity.ToTable("player_rankings");
            entity.HasKey(r => r.UserId);
        });
    }
}
