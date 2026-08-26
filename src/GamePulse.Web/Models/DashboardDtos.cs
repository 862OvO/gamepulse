namespace GamePulse.Web.Models;

public sealed record DashboardSummaryDto(
    int EventCount,
    int PlayerCount,
    int GameCount,
    double AverageDurationSeconds,
    double AverageScore);

public sealed record RetentionReportDto(
    int EligiblePlayers,
    int RetainedPlayers,
    double DayOneRetentionRate);

public sealed record FunnelStageDto(
    string Name,
    int Sessions,
    double ConversionRate);

public sealed record FunnelReportDto(
    int TargetScore,
    int StartedSessions,
    IReadOnlyList<FunnelStageDto> Stages);

public sealed record DailyTrendPointDto(
    DateOnly Date,
    int ActivePlayers,
    int Games,
    double AverageScore);

public sealed record ImportResultDto(
    int Received,
    int Imported,
    int Rejected,
    IReadOnlyList<string> Errors);

public sealed record DashboardSnapshot(
    DashboardSummaryDto Summary,
    RetentionReportDto Retention,
    FunnelReportDto Funnel,
    IReadOnlyList<DailyTrendPointDto> Trend);
