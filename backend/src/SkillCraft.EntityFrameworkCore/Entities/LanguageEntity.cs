using Logitar.EventSourcing;
using SkillCraft.Domain.Languages;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public string? Script { get; private set; }
  public string? TypicalSpeakers { get; private set; }

  public LanguageEntity(WorldEntity world, Language.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private LanguageEntity() : base()
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

  public void Update(Language.UpdatedEvent @event)
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

    if (@event.Script != null)
    {
      Script = @event.Script.Value?.Value;
    }
    if (@event.TypicalSpeakers != null)
    {
      TypicalSpeakers = @event.TypicalSpeakers.Value?.Value;
    }
  }
}
