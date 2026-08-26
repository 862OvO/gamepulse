using GamePulse.Application.Models;
using GamePulse.Domain.Entities;
using GamePulse.Domain.Enums;

namespace GamePulse.Application.Analytics;

public static class GameAnalytics
{
    public static DashboardSummary CalculateSummary(IEnumerable<GameEvent> source)
    {
        var events = source.ToArray();
        var gameOverEvents = events
            .Where(gameEvent => gameEvent.EventType == GameEventType.GameOver)
            .ToArray();

        return new DashboardSummary(
            events.Length,
            events.Select(gameEvent => gameEvent.PlayerId).Distinct().Count(),
            events.Where(gameEvent => gameEvent.EventType == GameEventType.GameStart)
                .Select(gameEvent => gameEvent.GameId)
                .Distinct()
                .Count(),
            Round(gameOverEvents.Where(gameEvent => gameEvent.DurationSeconds.HasValue)
                .Select(gameEvent => gameEvent.DurationSeconds!.Value)
                .DefaultIfEmpty()
                .Average()),
            Round(gameOverEvents.Where(gameEvent => gameEvent.Score.HasValue)
                .Select(gameEvent => gameEvent.Score!.Value)
                .DefaultIfEmpty()
                .Average()));
    }

    public static RetentionReport CalculateDayOneRetention(IEnumerable<GameEvent> source)
    {
        var activeDatesByPlayer = source
            .GroupBy(gameEvent => gameEvent.PlayerId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(gameEvent => gameEvent.OccurredAt.UtcDateTime.Date)
                    .ToHashSet());

        if (activeDatesByPlayer.Count == 0)
        {
            return new RetentionReport(0, 0, 0);
        }

        var latestObservedDate = activeDatesByPlayer.Values.SelectMany(dates => dates).Max();
        var eligiblePlayers = activeDatesByPlayer
            .Where(entry => entry.Value.Min() < latestObservedDate)
            .ToArray();
        var retainedPlayers = eligiblePlayers.Count(entry =>
        {
            var firstDate = entry.Value.Min();
            return entry.Value.Contains(firstDate.AddDays(1));
        });

        return new RetentionReport(
            eligiblePlayers.Length,
            retainedPlayers,
            eligiblePlayers.Length == 0
                ? 0
                : Round((double)retainedPlayers / eligiblePlayers.Length));
    }

    public static FunnelReport CalculateFunnel(IEnumerable<GameEvent> source, int targetScore)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetScore);

        var events = source.ToArray();
        var startedSessions = SessionIds(events, gameEvent =>
            gameEvent.EventType == GameEventType.GameStart);
        var movedSessions = SessionIds(events, gameEvent =>
            gameEvent.EventType == GameEventType.Move);
        var targetSessions = SessionIds(events, gameEvent =>
            gameEvent.Score >= targetScore);
        var finishedSessions = SessionIds(events, gameEvent =>
            gameEvent.EventType == GameEventType.GameOver);

        var startCount = startedSessions.Count;
        var stages = new[]
        {
            CreateStage("game_start", startedSessions.Count, startCount),
            CreateStage("move", movedSessions.Intersect(startedSessions).Count(), startCount),
            CreateStage("target_score", targetSessions.Intersect(startedSessions).Count(), startCount),
            CreateStage("game_over", finishedSessions.Intersect(startedSessions).Count(), startCount)
        };

        return new FunnelReport(targetScore, startCount, stages);
    }

    public static IReadOnlyList<DailyTrendPoint> CalculateDailyTrend(IEnumerable<GameEvent> source) =>
        source
            .GroupBy(gameEvent => DateOnly.FromDateTime(gameEvent.OccurredAt.UtcDateTime))
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var events = group.ToArray();
                var gameOverScores = events
                    .Where(gameEvent =>
                        gameEvent.EventType == GameEventType.GameOver &&
                        gameEvent.Score.HasValue)
                    .Select(gameEvent => gameEvent.Score!.Value)
                    .ToArray();

                return new DailyTrendPoint(
                    group.Key,
                    events.Select(gameEvent => gameEvent.PlayerId).Distinct().Count(),
                    events.Where(gameEvent => gameEvent.EventType == GameEventType.GameStart)
                        .Select(gameEvent => gameEvent.GameId)
                        .Distinct()
                        .Count(),
                    gameOverScores.Length == 0 ? 0 : Round(gameOverScores.Average()));
            })
            .ToArray();

    private static HashSet<string> SessionIds(
        IEnumerable<GameEvent> events,
        Func<GameEvent, bool> predicate) =>
        events.Where(predicate)
            .Select(gameEvent => gameEvent.SessionId)
            .ToHashSet(StringComparer.Ordinal);

    private static FunnelStage CreateStage(string name, int sessions, int startedSessions) =>
        new(name, sessions, startedSessions == 0 ? 0 : Round((double)sessions / startedSessions));

    private static double Round(double value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);
}
