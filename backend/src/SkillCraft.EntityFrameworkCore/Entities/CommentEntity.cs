using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Domain.Comments;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CommentEntity : AggregateEntity
{
  public int CommentId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public EntityType EntityType { get; private set; }
  public Guid EntityId { get; private set; }

  public string Text { get; private set; } = string.Empty;

  public CommentEntity(WorldEntity world, Comment.PostedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    EntityType = @event.EntityType;
    EntityId = @event.EntityId.ToGuid();

    SetText(@event.Text);
  }

  private CommentEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Edit(Comment.EditedEvent @event)
  {
    base.Update(@event);

    SetText(@event.Text);
  }

  private void SetText(Text text)
  {
    Text = text.Value;
  }
}
