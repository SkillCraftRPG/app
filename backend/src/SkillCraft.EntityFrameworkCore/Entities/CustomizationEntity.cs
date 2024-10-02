using Logitar.EventSourcing;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CustomizationEntity : AggregateEntity
{
  public int CustomizationId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public CustomizationType Type { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public List<PersonalityEntity> Personalities { get; private set; } = [];

  public CustomizationEntity(WorldEntity world, Customization.CreatedEvent @event) : base(@event)
  {
    Id = new CustomizationId(@event.AggregateId).EntityId;

    World = world;
    WorldId = world.WorldId;

    Type = @event.Type;

    Name = @event.Name.Value;
  }

  private CustomizationEntity() : base()
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

  public void Update(Customization.UpdatedEvent @event)
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
