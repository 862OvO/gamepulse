namespace GamePulse.Application.Models;

public sealed record DashboardSummary(
    int EventCount,
    int PlayerCount,
    int GameCount,
    double AverageDurationSeconds,
    double AverageScore);
