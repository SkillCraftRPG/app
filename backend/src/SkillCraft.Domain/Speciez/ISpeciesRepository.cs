namespace SkillCraft.Domain.Speciez;

public interface ISpeciesRepository
{
  Task<IReadOnlyCollection<Species>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Species?> LoadAsync(SpeciesId id, CancellationToken cancellationToken = default);
  Task<Species?> LoadAsync(SpeciesId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Species species, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Species> species, CancellationToken cancellationToken = default);
}
