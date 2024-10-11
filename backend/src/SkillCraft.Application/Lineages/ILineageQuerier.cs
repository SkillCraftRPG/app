using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Lineages;

public interface ILineageQuerier
{
  Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(LineageId id, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<LineageModel>> SearchAsync(WorldId worldId, SearchLineagesPayload payload, CancellationToken cancellationToken = default);
}
