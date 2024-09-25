using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storages;

[Trait(Traits.Category, Categories.Unit)]
public class StorageTests
{
  private const long AllocatedBytes = 5 * 1024 * 1024; // 5 MB
  private static readonly UserId _userId = UserId.NewId();

  private readonly EntityKey _entityKey = new(EntityType.World, Guid.NewGuid());
  private readonly Storage _storage = Storage.Initialize(_userId, AllocatedBytes);

  [Fact(DisplayName = "AvailableBytes: it should return the allocated bytes minus the used bytes.")]
  public void AvailableBytes_it_should_return_the_allocated_bytes_minus_the_used_bytes()
  {
    long size = 888;
    WorldId worldId = new(_entityKey.Id);
    _storage.Store(_entityKey, size, worldId);

    long availableBytes = AllocatedBytes - size;
    Assert.Equal(availableBytes, _storage.AvailableBytes);
  }

  [Fact(DisplayName = "GetSize: it should return 0 when the entity is not stored.")]
  public void GetSize_it_should_return_0_when_the_entity_is_not_stored()
  {
    Assert.Equal(0, _storage.GetSize(_entityKey));
  }

  [Fact(DisplayName = "GetSize: it should return the stored entity size.")]
  public void GetSize_it_should_return_the_stored_entity_size()
  {
    long size = 209;
    WorldId worldId = new(_entityKey.Id);
    _storage.Store(_entityKey, size, worldId);

    Assert.Equal(size, _storage.GetSize(_entityKey));
  }

  [Fact(DisplayName = "Initialize: it should initialize the correct storage.")]
  public void Initialize_it_should_initialize_the_correct_storage()
  {
    Assert.Equal(_userId, _storage.UserId);
    Assert.Equal(AllocatedBytes, _storage.AllocatedBytes);
  }

  [Fact(DisplayName = "Initialize: it should throw ArgumentOutOfRangeException when the allocated bytes are negative.")]
  public void Initialize_it_should_throw_ArgumentOutOfRangeException_when_the_allocated_bytes_are_negative()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Storage.Initialize(UserId.NewId(), -AllocatedBytes));
    Assert.Equal("allocatedBytes", exception.ParamName);
  }

  [Fact(DisplayName = "Store: it should not do anything when the entity has not changed.")]
  public void Store_it_should_not_do_anything_when_the_entity_has_not_changed()
  {
    long size = 104;
    WorldId worldId = new(_entityKey.Id);
    _storage.Store(_entityKey, size, worldId);
    _storage.ClearChanges();

    _storage.Store(_entityKey, size, worldId);
    Assert.False(_storage.HasChanges);
    Assert.Empty(_storage.Changes);
  }

  [Fact(DisplayName = "Store: it should store the specified entity.")]
  public void Store_it_should_store_the_specified_entity()
  {
    long size = 104;
    WorldId worldId = new(_entityKey.Id);

    _storage.Store(_entityKey, size, worldId);

    FieldInfo? _storedEntities = _storage.GetType().GetField("_storedEntities", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(_storedEntities);
    Dictionary<EntityKey, StoredEntity>? storedEntities = _storedEntities.GetValue(_storage) as Dictionary<EntityKey, StoredEntity>;
    Assert.NotNull(storedEntities);

    KeyValuePair<EntityKey, StoredEntity> storedEntity = Assert.Single(storedEntities);
    Assert.Equal(_entityKey, storedEntity.Key);
    Assert.Equal(size, storedEntity.Value.Size);
    Assert.Equal(worldId, storedEntity.Value.WorldId);
  }

  [Fact(DisplayName = "UsedBytes: it should return the sum of used bytes.")]
  public void UsedBytes_it_should_return_the_sum_of_used_bytes()
  {
    Assert.Equal(0, _storage.UsedBytes);

    long size1 = 99;
    long size2 = 901;
    long total = size1 + size2;
    WorldId worldId = new(_entityKey.Id);
    EntityKey entityKey = new(EntityType.World, Guid.NewGuid());
    _storage.Store(_entityKey, size1, worldId);
    _storage.Store(entityKey, size2, worldId);

    Assert.Equal(total, _storage.UsedBytes);
  }
}
