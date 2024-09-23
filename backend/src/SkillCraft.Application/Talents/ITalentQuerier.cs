using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents;

public interface ITalentQuerier
{
  Task<TalentId?> FindIdAsync(WorldId worldId, Skill skill, CancellationToken cancellationToken = default);

  Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken = default);
  Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<TalentModel>> SearchAsync(WorldId worldId, SearchTalentsPayload payload, CancellationToken cancellationToken = default);
}
