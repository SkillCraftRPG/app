using Logitar.EventSourcing;
using SkillCraft.Domain.Parties;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class PartyEntity : AggregateEntity
{
  public int PartyId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public PartyEntity(WorldEntity world, Party.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private PartyEntity() : base()
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

  public void Update(Party.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Name != null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
