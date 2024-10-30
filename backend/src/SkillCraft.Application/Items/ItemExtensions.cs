using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Items;

internal static class ItemExtensions
{
  public static EntityMetadata GetMetadata(this Item item) => new(item.WorldId, new EntityKey(EntityType.Item, item.EntityId), item.CalculateSize());
  public static EntityMetadata GetMetadata(this ItemModel item)
  {
    // TODO(fpion): remove this method from all entity extensions
    return new EntityMetadata(new WorldId(item.World.Id), new EntityKey(EntityType.Item, item.Id), size: 0);
  }

  private static long CalculateSize(this Item item) => item.Name.Size + (item.Description?.Size ?? 0)
    + 8 /* Value */ + 8 /* Weight */ + 1 /* IsAttunementRequired */
    + 4 /* ItemCategory */ + item.Properties.Size;
}
