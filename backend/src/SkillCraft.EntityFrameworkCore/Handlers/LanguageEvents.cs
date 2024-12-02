using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Languages;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal class LanguageEvents : INotificationHandler<Language.CreatedEvent>, INotificationHandler<Language.UpdatedEvent>
{
  private readonly SkillCraftContext _context;

  public LanguageEvents(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(Language.CreatedEvent @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (language == null)
    {
      Guid worldId = new LanguageId(@event.AggregateId).WorldId.ToGuid();
      WorldEntity world = await _context.Worlds
        .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

      language = new(world, @event);

      _context.Languages.Add(language);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(Language.UpdatedEvent @event, CancellationToken cancellationToken)
  {
    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.AggregateId}' could not be found.");

    language.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
