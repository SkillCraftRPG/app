using Logitar.EventSourcing;
using SkillCraft.Domain.Storage.Events;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class StorageAggregateTests
{
  private readonly Random _random = new();

  [Fact(DisplayName = "AvailableBytes: it should return the allocated bytes minus the used bytes.")]
  public void AvailableBytes_it_should_return_the_allocated_bytes_minus_the_used_bytes()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId otherId = WorldId.NewId();
    StoredEntityMock otherEntity = new(otherId, EntityType.World, otherId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 4));
    storage.Store(otherEntity);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock currentEntity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));
    storage.Store(currentEntity);

    Assert.Equal(storage.AllocatedBytes - storage.UsedBytes, storage.AvailableBytes);
  }

  [Fact(DisplayName = "GetSize: it should return 0 when the entity is not stored.")]
  public void GetSize_it_should_return_0_when_the_entity_is_not_stored()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock entity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));

    Assert.Equal(0, storage.GetSize(entity));
    Assert.Equal(0, storage.GetSize(entity.EntityType, entity.EntityId));
  }

  [Fact(DisplayName = "GetSize: it should return the entity size when it is stored.")]
  public void GetSize_it_should_return_the_entity_size_when_it_is_stored()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock entity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));
    storage.Store(entity);

    Assert.Equal(entity.Size, storage.GetSize(entity));
    Assert.Equal(entity.Size, storage.GetSize(entity.EntityType, entity.EntityId));
  }

  [Theory(DisplayName = "Initialize: it should create a new instance.")]
  [InlineData("15fa8d9f-81f8-4662-b29b-427e85bfbc78", 1024)]
  public void Initialize_it_should_create_a_new_instance(string userIdRaw, long allocatedBytes)
  {
    Guid userId = Guid.Parse(userIdRaw);
    StorageAggregate storage = StorageAggregate.Initialize(userId, allocatedBytes);

    Assert.Equal(userId, storage.Id.ToGuid());
    Assert.Equal(userId, storage.UserId);
    Assert.Equal(allocatedBytes, storage.AllocatedBytes);

    DomainEvent change = Assert.Single(storage.Changes);
    Assert.True(change is StorageInitializedEvent e && e.UserId == userId && e.AllocatedBytes == allocatedBytes && e.ActorId.ToGuid() == userId);
  }

  [Theory(DisplayName = "Initialize: it should throw ArgumentException when the allocated bytes are less than one.")]
  [InlineData(0)]
  [InlineData(-128)]
  public void Initialize_it_should_throw_ArgumentException_when_the_allocated_bytes_are_less_than_1(long allocatedBytes)
  {
    Assert.True(allocatedBytes < 1);

    var exception = Assert.Throws<ArgumentException>(() => StorageAggregate.Initialize(Guid.Empty, allocatedBytes: 1024));
    Assert.StartsWith("The user identifier is required.", exception.Message);
    Assert.Equal("userId", exception.ParamName);
  }

  [Fact(DisplayName = "Initialize: it should throw ArgumentException when the user ID is empty.")]
  public void Initialize_it_should_throw_ArgumentException_when_the_user_Id_is_empty()
  {
    var exception = Assert.Throws<ArgumentException>(() => StorageAggregate.Initialize(Guid.Empty, allocatedBytes: 1024));
    Assert.StartsWith("The user identifier is required.", exception.Message);
    Assert.Equal("userId", exception.ParamName);
  }

  [Fact(DisplayName = "Store: it should not do anything when the entity size has not changed.")]
  public void Store_it_should_not_do_anything_when_the_entity_size_has_not_changed()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock entity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes));
    storage.Store(entity);

    storage.ClearChanges();

    storage.Store(entity);

    Assert.Empty(storage.Changes);
  }

  [Fact(DisplayName = "Store: it should create a stored entity when it does not exist.")]
  public void Store_it_should_create_a_stored_entity_when_it_does_not_exist()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);
    storage.ClearChanges();

    WorldId worldId = WorldId.NewId();
    StoredEntityMock entity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));
    storage.Store(entity);

    DomainEvent change = Assert.Single(storage.Changes);
    Assert.True(change is EntityStoredEvent e && e.WorldId == entity.WorldId && e.EntityType == entity.EntityType
      && e.EntityId == entity.EntityId && e.Size == entity.Size && e.UsedBytes == entity.Size
      && e.ActorId.ToGuid() == storage.UserId);
  }

  [Fact(DisplayName = "Store: it should update the stored entity when its size has changed.")]
  public void Store_it_should_update_the_stored_entity_when_its_size_has_changed()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId otherId = WorldId.NewId();
    StoredEntityMock otherEntity = new(otherId, EntityType.World, otherId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 4));
    storage.Store(otherEntity);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock previousEntity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));
    storage.Store(previousEntity);

    storage.ClearChanges();

    StoredEntityMock currentEntity = new(previousEntity.WorldId, previousEntity.EntityType, previousEntity.EntityId, previousEntity.Size * 3 / 2);
    storage.Store(currentEntity);

    long usedBytes = otherEntity.Size + currentEntity.Size;
    DomainEvent change = Assert.Single(storage.Changes);
    Assert.True(change is EntityStoredEvent e && e.WorldId == currentEntity.WorldId && e.EntityType == currentEntity.EntityType
      && e.EntityId == currentEntity.EntityId && e.Size == currentEntity.Size && e.UsedBytes == usedBytes
      && e.ActorId.ToGuid() == storage.UserId);
  }

  [Fact(DisplayName = "UsedBytes: it should return the total size of stored entities.")]
  public void UsedBytes_it_should_return_the_total_size_of_stored_entities()
  {
    StorageAggregate storage = StorageAggregate.Initialize(Guid.NewGuid(), allocatedBytes: 1024);

    WorldId otherId = WorldId.NewId();
    StoredEntityMock otherEntity = new(otherId, EntityType.World, otherId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 4));
    storage.Store(otherEntity);

    WorldId worldId = WorldId.NewId();
    StoredEntityMock currentEntity = new(worldId, EntityType.World, worldId.ToGuid(), _random.Next(1, (int)storage.AvailableBytes / 2));
    storage.Store(currentEntity);

    Assert.Equal(otherEntity.Size + currentEntity.Size, storage.UsedBytes);
  }
}
