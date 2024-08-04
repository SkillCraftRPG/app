using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

public interface IWorldRepository
{
  Task<WorldAggregate?> LoadAsync(SlugUnit slug, CancellationToken cancellationToken = default);

  Task SaveAsync(WorldAggregate world, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<WorldAggregate> worlds, CancellationToken cancellationToken = default);
}
