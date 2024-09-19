using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
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

  public AttributeSelectionModel GetAttributes() => new()
  {
    Mandatory1 = MandatoryAttribute1,
    Mandatory2 = MandatoryAttribute2,
    Optional1 = OptionalAttribute1,
    Optional2 = OptionalAttribute2
  };
  public SkillsModel GetSkills() => new()
  {
    Discounted1 = DiscountedSkill1,
    Discounted2 = DiscountedSkill2
  };

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
      SetAttributes(@event.Attributes);
    }
    if (@event.Skills != null)
    {
      SetSkills(@event.Skills);
    }
  }
  private void SetAttributes(IAttributeSelection attributes)
  {
    MandatoryAttribute1 = attributes.Mandatory1;
    MandatoryAttribute2 = attributes.Mandatory2;
    OptionalAttribute1 = attributes.Optional1;
    OptionalAttribute2 = attributes.Optional2;
  }
  private void SetSkills(ISkills skills)
  {
    DiscountedSkill1 = skills.Discounted1;
    DiscountedSkill2 = skills.Discounted2;
  }
}
