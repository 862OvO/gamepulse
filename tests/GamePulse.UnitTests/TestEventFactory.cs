using GamePulse.Domain.Entities;
using GamePulse.Domain.Enums;

namespace GamePulse.UnitTests;

internal static class TestEventFactory
{
    public static IReadOnlyList<GameEvent> CreateAnalyticsDataset() =>
    [
        Create("player-a", "session-a", "game-a", GameEventType.GameStart, "2026-08-01T08:00:00Z"),
        Create("player-a", "session-a", "game-a", GameEventType.Move, "2026-08-01T08:00:10Z", 32),
        Create("player-a", "session-a", "game-a", GameEventType.GameOver, "2026-08-01T08:02:00Z", 512, 120),
        Create("player-b", "session-b", "game-b", GameEventType.GameStart, "2026-08-01T09:00:00Z"),
        Create("player-b", "session-b", "game-b", GameEventType.Move, "2026-08-01T09:00:10Z", 16),
        Create("player-b", "session-b", "game-b", GameEventType.GameOver, "2026-08-01T09:01:20Z", 64, 80),
        Create("player-a", "session-c", "game-c", GameEventType.SessionStart, "2026-08-02T08:00:00Z")
    ];

    public static GameEvent Create(
        string playerId,
        string sessionId,
        string gameId,
        GameEventType eventType,
        string occurredAt,
        int? score = null,
        int? durationSeconds = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            SessionId = sessionId,
            GameId = gameId,
            EventType = eventType,
            OccurredAt = DateTimeOffset.Parse(occurredAt),
            GameVersion = "1.0.0",
            DeviceType = "simulator",
            Score = score,
            DurationSeconds = durationSeconds
        };
}
