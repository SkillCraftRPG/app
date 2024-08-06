using Logitar.EventSourcing;
using SkillCraft.Domain;
using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageDetailEntity
{
  public int StorageDetailId { get; private set; }

  public Guid UserId { get; private set; }
  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public EntityType EntityType { get; private set; }
  public Guid EntityId { get; private set; }

  public long Size { get; private set; }

  public StorageDetailEntity(WorldEntity world, EntityStoredEvent @event)
  {
    UserId = new ActorId(world.OwnerId).ToGuid();
    World = world;
    WorldId = world.WorldId;

    EntityType = @event.EntityType;
    EntityId = @event.EntityId;

    Update(@event);
  }

  private StorageDetailEntity()
  {
  }

  public void Update(EntityStoredEvent @event)
  {
    Size = @event.Size;
  }
}
