using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents;

public interface ITalentQuerier
{
  Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<TalentModel>> SearchAsync(WorldId worldId, SearchTalentsPayload payload, CancellationToken cancellationToken = default);
}
