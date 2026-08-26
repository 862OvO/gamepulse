using GamePulse.Application.Analytics;

namespace GamePulse.UnitTests;

public sealed class GameAnalyticsTests
{
    [Fact]
    public void CalculateSummary_ReturnsStableMetrics()
    {
        var summary = GameAnalytics.CalculateSummary(TestEventFactory.CreateAnalyticsDataset());

        Assert.Equal(7, summary.EventCount);
        Assert.Equal(2, summary.PlayerCount);
        Assert.Equal(2, summary.GameCount);
        Assert.Equal(100, summary.AverageDurationSeconds);
        Assert.Equal(288, summary.AverageScore);
    }

    [Fact]
    public void CalculateDayOneRetention_UsesOnlyPlayersWithObservableNextDay()
    {
        var report = GameAnalytics.CalculateDayOneRetention(TestEventFactory.CreateAnalyticsDataset());

        Assert.Equal(2, report.EligiblePlayers);
        Assert.Equal(1, report.RetainedPlayers);
        Assert.Equal(0.5, report.DayOneRetentionRate);
    }

    [Fact]
    public void CalculateFunnel_ReturnsConversionFromStartedSessions()
    {
        var report = GameAnalytics.CalculateFunnel(
            TestEventFactory.CreateAnalyticsDataset(),
            targetScore: 512);

        Assert.Equal(2, report.StartedSessions);
        Assert.Collection(
            report.Stages,
            stage => Assert.Equal(("game_start", 2, 1d), (stage.Name, stage.Sessions, stage.ConversionRate)),
            stage => Assert.Equal(("move", 2, 1d), (stage.Name, stage.Sessions, stage.ConversionRate)),
            stage => Assert.Equal(("target_score", 1, 0.5d), (stage.Name, stage.Sessions, stage.ConversionRate)),
            stage => Assert.Equal(("game_over", 2, 1d), (stage.Name, stage.Sessions, stage.ConversionRate)));
    }

    [Fact]
    public void CalculateFunnel_RejectsNonPositiveTargetScore()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            GameAnalytics.CalculateFunnel(Array.Empty<GamePulse.Domain.Entities.GameEvent>(), 0));
    }

    [Fact]
    public void CalculateDailyTrend_UsesObservedDatesAndRealMetrics()
    {
        var trend = GameAnalytics.CalculateDailyTrend(TestEventFactory.CreateAnalyticsDataset());

        Assert.Collection(
            trend,
            point => Assert.Equal(
                (new DateOnly(2026, 8, 1), 2, 2, 288d),
                (point.Date, point.ActivePlayers, point.Games, point.AverageScore)),
            point => Assert.Equal(
                (new DateOnly(2026, 8, 2), 1, 0, 0d),
                (point.Date, point.ActivePlayers, point.Games, point.AverageScore)));
    }
}
