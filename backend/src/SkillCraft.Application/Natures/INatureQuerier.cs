using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Natures;

public interface INatureQuerier
{
  Task<NatureModel> ReadAsync(Nature nature, CancellationToken cancellationToken = default);
  Task<NatureModel?> ReadAsync(NatureId id, CancellationToken cancellationToken = default);
  Task<NatureModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<NatureModel>> SearchAsync(WorldId worldId, SearchNaturesPayload payload, CancellationToken cancellationToken = default);
}
