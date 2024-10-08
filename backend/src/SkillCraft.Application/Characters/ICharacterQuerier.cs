﻿using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters;

public interface ICharacterQuerier
{
  Task<CharacterModel> ReadAsync(Character character, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(CharacterId id, CancellationToken cancellationToken = default);
  Task<CharacterModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken = default);
}
