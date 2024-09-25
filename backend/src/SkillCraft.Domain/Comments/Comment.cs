using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Comments;

public class Comment : AggregateRoot
{
  public new CommentId Id => new(base.Id);

  public WorldId WorldId { get; private set; }
  public UserId OwnerId => new(CreatedBy);

  public EntityType EntityType { get; private set; }
  public AggregateId EntityId { get; private set; }
  public EntityKey Entity => new(EntityType, EntityId.ToGuid());

  private Text? _text = null;
  public Text Text => _text ?? throw new InvalidOperationException($"The {nameof(Text)} has not been initialized yet.");

  public Comment() : base()
  {
  }

  private Comment(WorldId worldId, EntityKey entity, Text text, UserId userId, CommentId? id = null) : base(id?.AggregateId)
  {
    AggregateId entityId = new(entity.Id);
    Raise(new PostedEvent(worldId, entity.Type, entityId, text), userId.ActorId);
  }
  protected virtual void Apply(PostedEvent @event)
  {
    WorldId = @event.WorldId;

    EntityType = @event.EntityType;
    EntityId = @event.EntityId;

    _text = @event.Text;
  }

  public static Comment Post(WorldId worldId, EntityKey entity, Text text, UserId userId, CommentId? id = null)
  {
    return new Comment(worldId, entity, text, userId, id);
  }

  public void Edit(Text text, UserId userId)
  {
    if (_text != text)
    {
      Raise(new EditedEvent(text), userId.ActorId);
    }
  }
  protected virtual void Apply(EditedEvent @event)
  {
    _text = @event.Text;
  }

  public class PostedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public EntityType EntityType { get; }
    public AggregateId EntityId { get; }

    public Text Text { get; }

    public PostedEvent(WorldId worldId, EntityType entityType, AggregateId entityId, Text text)
    {
      WorldId = worldId;

      EntityType = entityType;
      EntityId = entityId;

      Text = text;
    }
  }

  public class EditedEvent : DomainEvent, INotification
  {
    public Text Text { get; }

    public EditedEvent(Text text)
    {
      Text = text;
    }
  }
}
