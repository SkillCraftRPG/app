using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Comments;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class CommentEvents
{
  public class EditedEventHandler : INotificationHandler<Comment.EditedEvent>
  {
    private readonly SkillCraftContext _context;

    public EditedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Comment.EditedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      CommentEntity comment = await _context.Comments
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The comment entity 'Id={id}' could not be found.");

      comment.Edit(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class PostedEventHandler : INotificationHandler<Comment.PostedEvent>
  {
    private readonly SkillCraftContext _context;

    public PostedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Comment.PostedEvent @event, CancellationToken cancellationToken)
    {
      CommentEntity? comment = await _context.Comments.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (comment == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        comment = new(world, @event);

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}
