using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Domain.Castes;
using System.Text.Json;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CasteEntity : AggregateEntity
{
  public int CasteId { get; private set; }
  public Guid Id { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public Skill? Skill { get; private set; }
  public string? WealthRoll { get; private set; }

  public Dictionary<Guid, TraitEntity> Traits { get; private set; } = [];
  public string? TraitsSerialized
  {
    get => Traits.Count == 0 ? null : JsonSerializer.Serialize(Traits);
    private set
    {
      Traits.Clear();
      if (value != null)
      {
        Dictionary<Guid, TraitEntity>? traits = JsonSerializer.Deserialize<Dictionary<Guid, TraitEntity>>(value);
        if (traits != null)
        {
          foreach (KeyValuePair<Guid, TraitEntity> trait in traits)
          {
            Traits[trait.Key] = trait.Value;
          }
        }
      }
    }
  }

  public CasteEntity(WorldEntity world, Caste.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    World = world;
    WorldId = world.WorldId;

    Name = @event.Name.Value;
  }

  private CasteEntity() : base()
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

  public void Update(Caste.UpdatedEvent @event)
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
    if (@event.WealthRoll != null)
    {
      WealthRoll = @event.WealthRoll.Value?.Value;
    }

    // TODO(fpion): Traits
  }
}
