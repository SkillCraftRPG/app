using Logitar.EventSourcing;
using SkillCraft.Domain.Worlds.Events;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class WorldEntity : AggregateEntity
{
  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => SkillCraftDb.Normalize(UniqueSlug);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public List<StorageDetailEntity> StorageDetails { get; private set; } = [];

  private WorldEntity() : base()
  {
  }

  public WorldEntity(WorldCreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    OwnerId = @event.ActorId.Value;

    UniqueSlug = @event.UniqueSlug.Value;
  }

  public override IEnumerable<ActorId> GetActorIds() => base.GetActorIds().Concat([new(OwnerId)]);

  public void Update(WorldUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.UniqueSlug != null)
    {
      UniqueSlug = @event.UniqueSlug.Value;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
