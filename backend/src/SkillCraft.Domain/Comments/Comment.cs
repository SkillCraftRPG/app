using Logitar.EventSourcing;
using MediatR;

namespace SkillCraft.Domain.Comments;

public class Comment : AggregateRoot
{
  public new CommentId Id => new(base.Id);

  private Text? _text = null;
  public Text Text => _text ?? throw new InvalidOperationException($"The {nameof(Text)} has not been initialized yet.");

  public Comment(Text text, UserId userId, CommentId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(text), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _text = @event.Text;
  }

  public class CreatedEvent : DomainEvent, INotification
  {
    public Text Text { get; }

    public CreatedEvent(Text text)
    {
      Text = text;
    }
  }
}
