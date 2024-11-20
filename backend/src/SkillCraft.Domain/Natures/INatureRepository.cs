namespace SkillCraft.Domain.Natures;

public interface INatureRepository
{
  Task<IReadOnlyCollection<Nature>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Nature?> LoadAsync(NatureId id, CancellationToken cancellationToken = default);
  Task<Nature?> LoadAsync(NatureId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Nature nature, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Nature> natures, CancellationToken cancellationToken = default);
}
