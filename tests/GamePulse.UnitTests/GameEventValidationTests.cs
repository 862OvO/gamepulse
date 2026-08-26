using GamePulse.Domain.Entities;
using GamePulse.Domain.Enums;

namespace GamePulse.UnitTests;

public sealed class GameEventValidationTests
{
    [Fact]
    public void Validate_RejectsNegativeMetricsAndInvalidDirection()
    {
        var gameEvent = new GameEvent
        {
            Id = Guid.NewGuid(),
            PlayerId = "anonymous-player",
            SessionId = "session-1",
            GameId = "game-1",
            EventType = GameEventType.Move,
            OccurredAt = DateTimeOffset.UtcNow,
            GameVersion = "1.0.0",
            DeviceType = "simulator",
            Score = -1,
            Direction = "diagonal"
        };

        var errors = gameEvent.Validate();

        Assert.Contains(errors, error => error.Contains("Score", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("Direction", StringComparison.Ordinal));
    }
}
