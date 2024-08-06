using SkillCraft.Domain;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageDetailEntity
{
  public int StorageDetailId { get; private set; }

  public Guid UserId { get; private set; }
  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }

  public EntityType EntityType { get; private set; }
  public Guid EntityId { get; private set; }

  public long UsedBytes { get; private set; }

  private StorageDetailEntity()
  {
  }
}
