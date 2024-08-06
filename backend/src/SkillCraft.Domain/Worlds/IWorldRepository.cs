namespace SkillCraft.Domain.Worlds;

public interface IWorldRepository
{
  Task<WorldAggregate?> LoadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<WorldAggregate?> LoadAsync(SlugUnit uniqueSlug, CancellationToken cancellationToken = default);

  Task SaveAsync(WorldAggregate world, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<WorldAggregate> worlds, CancellationToken cancellationToken = default);
}
