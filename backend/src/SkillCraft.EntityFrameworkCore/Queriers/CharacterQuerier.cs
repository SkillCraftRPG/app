using SkillCraft.Application.Characters;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CharacterQuerier : ICharacterQuerier
{
  public async Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken)
  {
    return await ReadAsync(character.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'AggregateId={character.Id}' could not be found.");
  }
  public async Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public Task<CharacterModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement character read
  }
}
