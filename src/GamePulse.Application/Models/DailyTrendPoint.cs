namespace GamePulse.Application.Models;

public sealed record DailyTrendPoint(
    DateOnly Date,
    int ActivePlayers,
    int Games,
    double AverageScore);
