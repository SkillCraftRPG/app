using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Domain.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class AspectEntity : AggregateEntity
{
  public int AspectId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public Attribute? MandatoryAttribute1 { get; private set; }
  public Attribute? MandatoryAttribute2 { get; private set; }
  public Attribute? OptionalAttribute1 { get; private set; }
  public Attribute? OptionalAttribute2 { get; private set; }

  public Skill? DiscountedSkill1 { get; private set; }
  public Skill? DiscountedSkill2 { get; private set; }

  public AspectEntity(WorldEntity world, Aspect.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private AspectEntity() : base()
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

  public void Update(Aspect.UpdatedEvent @event)
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

    if (@event.Attributes != null)
    {
      MandatoryAttribute1 = @event.Attributes.Mandatory1;
      MandatoryAttribute2 = @event.Attributes.Mandatory2;
      OptionalAttribute1 = @event.Attributes.Optional1;
      OptionalAttribute2 = @event.Attributes.Optional2;
    }
    if (@event.Skills != null)
    {
      DiscountedSkill1 = @event.Skills.Discounted1;
      DiscountedSkill2 = @event.Skills.Discounted2;
    }
  }
}
