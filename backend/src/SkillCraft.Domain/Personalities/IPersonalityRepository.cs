namespace SkillCraft.Domain.Personalities;

public interface IPersonalityRepository
{
  Task<IReadOnlyCollection<Personality>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Personality?> LoadAsync(PersonalityId id, CancellationToken cancellationToken = default);
  Task<Personality?> LoadAsync(PersonalityId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Personality personality, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Personality> personalities, CancellationToken cancellationToken = default);
}
