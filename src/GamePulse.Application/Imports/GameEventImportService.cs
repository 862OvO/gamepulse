using GamePulse.Application.Abstractions;
using GamePulse.Application.Models;
using GamePulse.Domain.Entities;

namespace GamePulse.Application.Imports;

public sealed class GameEventImportService(IGameEventRepository repository)
{
    public async Task<ImportResult> ImportAsync(
        IReadOnlyCollection<GameEvent> events,
        CancellationToken cancellationToken = default)
    {
        if (events.Count == 0)
        {
            return new ImportResult(0, 0, 0, Array.Empty<string>());
        }

        var errors = new List<string>();
        var accepted = new List<GameEvent>();
        var duplicateIds = events
            .GroupBy(gameEvent => gameEvent.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet();
        var existingIds = await repository.GetExistingIdsAsync(
            events.Select(gameEvent => gameEvent.Id),
            cancellationToken);

        foreach (var (gameEvent, index) in events.Select((value, index) => (value, index)))
        {
            var validationErrors = gameEvent.Validate();
            if (validationErrors.Count > 0)
            {
                errors.Add($"第 {index + 1} 条：{string.Join('；', validationErrors)}");
                continue;
            }

            if (duplicateIds.Contains(gameEvent.Id))
            {
                errors.Add($"第 {index + 1} 条：批次内存在重复 EventId {gameEvent.Id}。");
                continue;
            }

            if (existingIds.Contains(gameEvent.Id))
            {
                errors.Add($"第 {index + 1} 条：EventId {gameEvent.Id} 已导入。");
                continue;
            }

            accepted.Add(gameEvent);
        }

        if (accepted.Count > 0)
        {
            await repository.AddRangeAsync(accepted, cancellationToken);
        }

        return new ImportResult(events.Count, accepted.Count, events.Count - accepted.Count, errors);
    }
}
