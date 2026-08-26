using GamePulse.Domain.Entities;
using GamePulse.Domain.Enums;
using GamePulse.Infrastructure.Data;
using GamePulse.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GamePulse.IntegrationTests;

public sealed class RepositoryCompatibilityTests
{
    [Fact]
    public async Task ListAsync_OrdersDateTimeOffsetValuesWithSqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<GamePulseDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new GamePulseDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var laterEvent = CreateEvent(DateTimeOffset.Parse("2026-08-02T08:00:00+08:00"));
        var earlierEvent = CreateEvent(DateTimeOffset.Parse("2026-08-01T08:00:00+08:00"));
        await dbContext.GameEvents.AddRangeAsync(laterEvent, earlierEvent);
        await dbContext.SaveChangesAsync();

        var repository = new GameEventRepository(dbContext);
        var events = await repository.ListAsync();

        Assert.Equal([earlierEvent.Id, laterEvent.Id], events.Select(gameEvent => gameEvent.Id));
    }

    private static GameEvent CreateEvent(DateTimeOffset occurredAt) => new()
    {
        Id = Guid.NewGuid(),
        PlayerId = "player-1",
        SessionId = Guid.NewGuid().ToString(),
        GameId = "fruit-2048",
        EventType = GameEventType.GameStart,
        OccurredAt = occurredAt,
        GameVersion = "1.0.0",
        DeviceType = "simulator",
    };
}
