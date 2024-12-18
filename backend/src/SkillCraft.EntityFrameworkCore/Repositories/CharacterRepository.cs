﻿using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class CharacterRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ICharacterRepository
{
  public CharacterRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Character>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Character>(cancellationToken)).ToArray();
  }

  public async Task<Character?> LoadAsync(CharacterId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Character?> LoadAsync(CharacterId id, long? version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Character>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Character character, CancellationToken cancellationToken)
  {
    await base.SaveAsync(character, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Character> characters, CancellationToken cancellationToken)
  {
    await base.SaveAsync(characters, cancellationToken);
  }
}
