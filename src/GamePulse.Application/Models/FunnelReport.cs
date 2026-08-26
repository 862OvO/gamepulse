namespace GamePulse.Application.Models;

public sealed record FunnelStage(
    string Name,
    int Sessions,
    double ConversionRate);

public sealed record FunnelReport(
    int TargetScore,
    int StartedSessions,
    IReadOnlyList<FunnelStage> Stages);
