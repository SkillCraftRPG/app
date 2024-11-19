using Logitar.EventSourcing;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Castes;

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

  public string? Features { get; private set; }

  public List<CharacterEntity> Characters { get; private set; } = [];

  public CasteEntity(WorldEntity world, Caste.CreatedEvent @event) : base(@event)
  {
    Id = new CasteId(@event.AggregateId).EntityId;

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

  public List<FeatureModel> GetFeatures()
  {
    Dictionary<Guid, FeatureEntity> features = DeserializeFeatures();
    return [.. features.Select(feature => new FeatureModel(feature.Value.Name)
    {
      Id = feature.Key,
      Description = feature.Value.Description
    }).OrderBy(x => x.Name)];
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

    if (@event.Features.Count > 0)
    {
      SetFeatures(@event.Features);
    }
  }
  private void SetFeatures(Dictionary<Guid, Feature?> features)
  {
    Dictionary<Guid, FeatureEntity> entities = DeserializeFeatures();
    foreach (KeyValuePair<Guid, Feature?> feature in features)
    {
      if (feature.Value == null)
      {
        entities.Remove(feature.Key);
      }
      else
      {
        entities[feature.Key] = new FeatureEntity(feature.Value.Name.Value, feature.Value.Description?.Value);
      }
    }
    Features = entities.Count == 0 ? null : JsonSerializer.Serialize(entities);
  }

  private Dictionary<Guid, FeatureEntity> DeserializeFeatures()
  {
    return (Features == null ? null : JsonSerializer.Deserialize<Dictionary<Guid, FeatureEntity>>(Features)) ?? [];
  }
}
