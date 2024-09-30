using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Aspects;

public interface IAspectQuerier
{
  Task<AspectModel> ReadAsync(Aspect aspect, CancellationToken cancellationToken = default);
  Task<AspectModel?> ReadAsync(AspectId id, CancellationToken cancellationToken = default);
  Task<AspectModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<AspectModel>> SearchAsync(WorldId worldId, SearchAspectsPayload payload, CancellationToken cancellationToken = default);
}
