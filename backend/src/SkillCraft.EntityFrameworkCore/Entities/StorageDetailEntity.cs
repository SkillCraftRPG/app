using SkillCraft.Domain;
using SkillCraft.Domain.Storages;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageDetailEntity
{
  public int StorageDetailId { get; private set; }

  public UserEntity? Owner { get; private set; }
  public int UserId { get; private set; }
  public Guid OwnerId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public EntityType EntityType { get; private set; }
  public Guid EntityId { get; private set; }

  public long Size { get; private set; }

  public StorageDetailEntity(WorldEntity world, Storage.EntityStoredEvent @event)
  {
    Owner = world.Owner;
    UserId = world.UserId;
    OwnerId = world.OwnerId;

    World = world;
    WorldId = world.WorldId;

    EntityType = @event.Key.Type;
    EntityId = @event.Key.Id;

    Update(@event);
  }

  private StorageDetailEntity()
  {
  }

  public void Update(Storage.EntityStoredEvent @event)
  {
    Size = @event.Entity.Size;
  }
}
