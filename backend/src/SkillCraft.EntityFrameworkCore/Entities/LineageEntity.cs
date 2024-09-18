using Logitar.EventSourcing;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class LineageEntity : AggregateEntity
{
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public LineageEntity? Species { get; private set; }
  public int? ParentId { get; private set; }
  public List<LineageEntity> Nations { get; private set; } = [];

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public LineageEntity(WorldEntity world, LineageEntity? parent, Lineage.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Species = parent;
    ParentId = parent?.ParentId;

    Name = @event.Name.Value;
  }

  private LineageEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (World != null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    if (Species != null)
    {
      actorIds.AddRange(Species.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Update(Lineage.UpdatedEvent @event)
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
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    // TODO(fpion): SetTraits

    if (@event.Languages != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    if (@event.Names != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    if (@event.Speeds != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    if (@event.Size != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    if (@event.Weight != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    if (@event.Ages != null)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
  }
}
