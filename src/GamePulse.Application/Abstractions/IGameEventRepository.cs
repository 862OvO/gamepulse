using GamePulse.Domain.Entities;

namespace GamePulse.Application.Abstractions;

public interface IGameEventRepository
{
    Task<IReadOnlyList<GameEvent>> ListAsync(CancellationToken cancellationToken = default);

    Task<HashSet<Guid>> GetExistingIdsAsync(
        IEnumerable<Guid> eventIds,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IReadOnlyCollection<GameEvent> events,
        CancellationToken cancellationToken = default);
}
