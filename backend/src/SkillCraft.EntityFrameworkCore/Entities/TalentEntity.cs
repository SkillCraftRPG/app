using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Domain.Talents;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class TalentEntity : AggregateEntity
{
  public int TalentId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public bool AllowMultiplePurchases { get; private set; }
  public TalentEntity? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }
  public List<TalentEntity> RequiringTalents { get; private set; } = [];
  public Skill? Skill { get; private set; }

  public TalentEntity(WorldEntity world, Talent.CreatedEvent @event) : base(@event)
  {
    Id = new TalentId(@event.AggregateId).EntityId;

    World = world;
    WorldId = world.WorldId;

    Tier = @event.Tier;

    Name = @event.Name.Value;

    Skill = @event.Skill;
  }

  private TalentEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    if (RequiredTalent != null)
    {
      actorIds.AddRange(RequiredTalent.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Update(Talent.UpdatedEvent @event, TalentEntity? requiredTalent)
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

    if (@event.AllowMultiplePurchases.HasValue)
    {
      AllowMultiplePurchases = @event.AllowMultiplePurchases.Value;
    }
    if (@event.RequiredTalentId != null)
    {
      RequiredTalent = requiredTalent;
      RequiredTalentId = requiredTalent?.TalentId;
    }
    if (@event.Skill != null)
    {
      Skill = @event.Skill.Value;
    }
  }
}
