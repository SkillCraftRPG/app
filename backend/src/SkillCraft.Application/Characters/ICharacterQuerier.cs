using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters;

public interface ICharacterQuerier
{
  Task<IReadOnlyCollection<Guid>> FindExistingAsync(WorldId worldId, IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<string>> ListPlayersAsync(WorldId worldId, CancellationToken cancellationToken = default);

  Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CharacterModel>> SearchAsync(WorldId worldId, SearchCharactersPayload payload, CancellationToken cancellationToken = default);
}
