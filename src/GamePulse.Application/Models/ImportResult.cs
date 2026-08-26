namespace GamePulse.Application.Models;

public sealed record ImportResult(
    int Received,
    int Imported,
    int Rejected,
    IReadOnlyList<string> Errors);
