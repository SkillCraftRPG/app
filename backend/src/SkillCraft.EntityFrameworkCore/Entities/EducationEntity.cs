using SkillCraft.Contracts;
using SkillCraft.Domain.Educations;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class EducationEntity : AggregateEntity
{
  public int EducationId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public Skill? Skill { get; private set; }
  public double? WealthMultiplier { get; private set; }

  public EducationEntity(WorldEntity world, Education.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private EducationEntity() : base()
  {
  }

  public void Update(Education.UpdatedEvent @event)
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

    if (@event.Skill != null)
    {
      Skill = @event.Skill.Value;
    }
    if (@event.WealthMultiplier != null)
    {
      WealthMultiplier = @event.WealthMultiplier.Value;
    }
  }
}
