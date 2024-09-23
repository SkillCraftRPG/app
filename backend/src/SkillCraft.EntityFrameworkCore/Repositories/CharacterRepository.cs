using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class CharacterRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ICharacterRepository
{
  public CharacterRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
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
