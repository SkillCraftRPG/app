using Logitar.EventSourcing;
using SkillCraft.Domain.Natures;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class NatureEntity : AggregateEntity
{
  public int NatureId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public Attribute? Attribute { get; private set; }
  public CustomizationEntity? Gift { get; private set; }
  public int? GiftId { get; private set; }

  public List<CharacterEntity> Characters { get; private set; } = [];

  public NatureEntity(WorldEntity world, Nature.CreatedEvent @event) : base(@event)
  {
    Id = new NatureId(@event.AggregateId).EntityId;

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private NatureEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    if (Gift != null)
    {
      actorIds.AddRange(Gift.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Update(Nature.UpdatedEvent @event, CustomizationEntity? gift)
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

    if (@event.Attribute != null)
    {
      Attribute = @event.Attribute.Value;
    }
    if (@event.GiftId != null)
    {
      Gift = gift;
      GiftId = gift?.CustomizationId;
    }
  }
}
