using GamePulse.Domain.Enums;

namespace GamePulse.Domain.Entities;

public sealed class GameEvent
{
    public Guid Id { get; set; }

    public required string PlayerId { get; set; }

    public required string SessionId { get; set; }

    public required string GameId { get; set; }

    public GameEventType EventType { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public required string GameVersion { get; set; }

    public required string DeviceType { get; set; }

    public int? Score { get; set; }

    public int? StepCount { get; set; }

    public int? DurationSeconds { get; set; }

    public int? FruitLevel { get; set; }

    public string? Direction { get; set; }

    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (Id == Guid.Empty)
        {
            errors.Add("EventId 不能为空。");
        }

        ValidateRequired(PlayerId, nameof(PlayerId), 64, errors);
        ValidateRequired(SessionId, nameof(SessionId), 64, errors);
        ValidateRequired(GameId, nameof(GameId), 64, errors);
        ValidateRequired(GameVersion, nameof(GameVersion), 32, errors);
        ValidateRequired(DeviceType, nameof(DeviceType), 32, errors);

        if (OccurredAt == default)
        {
            errors.Add("OccurredAt 不能为空。");
        }

        ValidateNonNegative(Score, nameof(Score), errors);
        ValidateNonNegative(StepCount, nameof(StepCount), errors);
        ValidateNonNegative(DurationSeconds, nameof(DurationSeconds), errors);
        ValidateNonNegative(FruitLevel, nameof(FruitLevel), errors);

        if (EventType == GameEventType.Move &&
            !string.IsNullOrWhiteSpace(Direction) &&
            !AllowedDirections.Contains(Direction))
        {
            errors.Add("Direction 仅支持 up、down、left 或 right。");
        }

        return errors;
    }

    private static readonly HashSet<string> AllowedDirections =
        new(StringComparer.OrdinalIgnoreCase) { "up", "down", "left", "right" };

    private static void ValidateRequired(
        string? value,
        string fieldName,
        int maximumLength,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{fieldName} 不能为空。");
        }
        else if (value.Length > maximumLength)
        {
            errors.Add($"{fieldName} 不能超过 {maximumLength} 个字符。");
        }
    }

    private static void ValidateNonNegative(
        int? value,
        string fieldName,
        ICollection<string> errors)
    {
        if (value < 0)
        {
            errors.Add($"{fieldName} 不能为负数。");
        }
    }
}
