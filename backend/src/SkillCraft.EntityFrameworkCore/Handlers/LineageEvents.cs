using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Lineages;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class LineageEvents
{
  public class CreatedEventHandler : INotificationHandler<Lineage.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Lineage.CreatedEvent @event, CancellationToken cancellationToken)
    {
      LineageEntity? lineage = await _context.Lineages.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (lineage == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        LineageEntity? parent = null;
        if (@event.ParentId.HasValue)
        {
          Guid parentId = @event.ParentId.Value.ToGuid();
          parent = await _context.Lineages
            .SingleOrDefaultAsync(x => x.Id == parentId, cancellationToken)
            ?? throw new InvalidOperationException($"The lineage entity 'Id={parentId}' could not be found.");
        }

        lineage = new(world, parent, @event);

        _context.Lineages.Add(lineage);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Lineage.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Lineage.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      LineageEntity lineage = await _context.Lineages
        .Include(x => x.Languages)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The lineage entity 'Id={id}' could not be found.");

      LanguageEntity[] languages = [];
      if (@event.Languages != null && @event.Languages.Ids.Count > 0)
      {
        HashSet<string> languageIds = @event.Languages.Ids.Select(id => id.Value).ToHashSet();
        languages = await _context.Languages
          .Where(x => languageIds.Contains(x.AggregateId))
          .ToArrayAsync(cancellationToken);
      }

      lineage.Update(@event, languages);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
