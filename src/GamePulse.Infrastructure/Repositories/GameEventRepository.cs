using GamePulse.Application.Abstractions;
using GamePulse.Domain.Entities;
using GamePulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GamePulse.Infrastructure.Repositories;

public sealed class GameEventRepository(GamePulseDbContext dbContext) : IGameEventRepository
{
    public async Task<IReadOnlyList<GameEvent>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var events = await dbContext.GameEvents
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return events
            .OrderBy(gameEvent => gameEvent.OccurredAt)
            .ToList();
    }

    public async Task<HashSet<Guid>> GetExistingIdsAsync(
        IEnumerable<Guid> eventIds,
        CancellationToken cancellationToken = default)
    {
        var ids = eventIds.Distinct().ToArray();
        var existingIds = await dbContext.GameEvents
            .Where(gameEvent => ids.Contains(gameEvent.Id))
            .Select(gameEvent => gameEvent.Id)
            .ToListAsync(cancellationToken);

        return existingIds.ToHashSet();
    }

    public async Task AddRangeAsync(
        IReadOnlyCollection<GameEvent> events,
        CancellationToken cancellationToken = default)
    {
        await dbContext.GameEvents.AddRangeAsync(events, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
