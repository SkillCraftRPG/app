namespace SkillCraft.Domain.Characters;

public interface ICharacterRepository
{
  Task SaveAsync(Character character, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken = default);
}
