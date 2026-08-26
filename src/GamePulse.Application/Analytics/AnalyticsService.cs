using GamePulse.Application.Abstractions;
using GamePulse.Application.Models;

namespace GamePulse.Application.Analytics;

public sealed class AnalyticsService(IGameEventRepository repository)
{
    public async Task<DashboardSummary> GetSummaryAsync(
        CancellationToken cancellationToken = default) =>
        GameAnalytics.CalculateSummary(await repository.ListAsync(cancellationToken));

    public async Task<RetentionReport> GetDayOneRetentionAsync(
        CancellationToken cancellationToken = default) =>
        GameAnalytics.CalculateDayOneRetention(await repository.ListAsync(cancellationToken));

    public async Task<FunnelReport> GetFunnelAsync(
        int targetScore,
        CancellationToken cancellationToken = default) =>
        GameAnalytics.CalculateFunnel(await repository.ListAsync(cancellationToken), targetScore);

    public async Task<IReadOnlyList<DailyTrendPoint>> GetDailyTrendAsync(
        CancellationToken cancellationToken = default) =>
        GameAnalytics.CalculateDailyTrend(await repository.ListAsync(cancellationToken));
}
