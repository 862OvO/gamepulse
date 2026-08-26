using GamePulse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamePulse.Infrastructure.Data;

public sealed class GamePulseDbContext(DbContextOptions<GamePulseDbContext> options)
    : DbContext(options)
{
    public DbSet<GameEvent> GameEvents => Set<GameEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var gameEvent = modelBuilder.Entity<GameEvent>();

        gameEvent.ToTable("game_events");
        gameEvent.HasKey(entity => entity.Id);
        gameEvent.Property(entity => entity.PlayerId).HasMaxLength(64).IsRequired();
        gameEvent.Property(entity => entity.SessionId).HasMaxLength(64).IsRequired();
        gameEvent.Property(entity => entity.GameId).HasMaxLength(64).IsRequired();
        gameEvent.Property(entity => entity.GameVersion).HasMaxLength(32).IsRequired();
        gameEvent.Property(entity => entity.DeviceType).HasMaxLength(32).IsRequired();
        gameEvent.Property(entity => entity.Direction).HasMaxLength(16);
        gameEvent.HasIndex(entity => new { entity.PlayerId, entity.OccurredAt });
        gameEvent.HasIndex(entity => new { entity.SessionId, entity.EventType });
    }
}
