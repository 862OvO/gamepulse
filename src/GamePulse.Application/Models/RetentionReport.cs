namespace GamePulse.Application.Models;

public sealed record RetentionReport(
    int EligiblePlayers,
    int RetainedPlayers,
    double DayOneRetentionRate);
