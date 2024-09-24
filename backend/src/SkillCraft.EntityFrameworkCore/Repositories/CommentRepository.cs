using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Comments;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class CommentRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ICommentRepository
{
  public CommentRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task SaveAsync(Comment comment, CancellationToken cancellationToken)
  {
    await base.SaveAsync(comment, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Comment> comments, CancellationToken cancellationToken)
  {
    await base.SaveAsync(comments, cancellationToken);
  }
}
