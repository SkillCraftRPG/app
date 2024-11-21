namespace SkillCraft.Domain.Characters;

public interface ICharacterRepository
{
  Task<IReadOnlyCollection<Character>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<Character?> LoadAsync(CharacterId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Character character, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken = default);
}
