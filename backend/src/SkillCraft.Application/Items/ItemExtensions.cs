using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Items;

internal static class ItemExtensions
{
  private const EntityType Type = EntityType.Item;

  public static EntityMetadata GetMetadata(this Item item)
  {
    long size = item.Name.Size + (item.Description?.Size ?? 0) + 8 + 8 + 1 + 4; // TODO(fpion): Properties
    return new EntityMetadata(item.WorldId, new EntityKey(Type, item.EntityId), size);
  }
  public static EntityMetadata GetMetadata(this ItemModel item)
  {
    // TODO(fpion): remove this method from all entity extensions
    return new EntityMetadata(new WorldId(item.World.Id), new EntityKey(Type, item.Id), size: 0);
  }
}
