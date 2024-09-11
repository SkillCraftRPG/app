namespace SkillCraft.Domain.Worlds;

public interface IWorldRepository
{
  Task<IReadOnlyCollection<World>> LoadAsync(CancellationToken cancellationToken = default);

  Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<World?> LoadAsync(WorldId id, long? version, CancellationToken cancellationToken = default);

  Task<World?> LoadAsync(Slug slug, CancellationToken cancellationToken = default);

  Task SaveAsync(World world, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken = default);
}
