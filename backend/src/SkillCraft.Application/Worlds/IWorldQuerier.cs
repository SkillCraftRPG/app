using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

public interface IWorldQuerier
{
  Task<int> CountOwnedAsync(User user, CancellationToken cancellationToken = default);

  Task<World> ReadAsync(WorldAggregate world, CancellationToken cancellationToken = default);
  Task<World?> ReadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<World?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<World>> SearchAsync(User user, SearchWorldsPayload payload, CancellationToken cancellationToken = default);
}
