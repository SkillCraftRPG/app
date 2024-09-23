using SkillCraft.Application.Characters;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CharacterQuerier : ICharacterQuerier
{
  public async Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken)
  {
    return await ReadAsync(character.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The character entity 'Id={character.Id.ToGuid()}' could not be found.");
  }
  public async Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public Task<CharacterModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
