using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Castes;

public interface ICasteQuerier
{
  Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(CasteId id, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CasteModel>> SearchAsync(WorldId worldId, SearchCastesPayload payload, CancellationToken cancellationToken = default);
}
