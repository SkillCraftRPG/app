using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

public interface ITalentRepository
{
  Task<IReadOnlyCollection<Talent>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken = default);
  Task<Talent?> LoadAsync(TalentId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Talent>> LoadAsync(IEnumerable<TalentId> ids, CancellationToken cancellationToken = default);

  Task<Talent?> LoadAsync(WorldId worldId, Skill skill, CancellationToken cancellationToken = default);

  Task SaveAsync(Talent talent, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken = default);
}
