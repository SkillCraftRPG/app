using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;

namespace SkillCraft.Application.Items;

internal static class ItemExtensions
{
  public static EntityMetadata GetMetadata(this Item item) => new(item.WorldId, new EntityKey(EntityType.Item, item.EntityId), item.CalculateSize());

  private static long CalculateSize(this Item item) => item.Name.Size + (item.Description?.Size ?? 0)
    + 8 /* Value */ + 8 /* Weight */ + 1 /* IsAttunementRequired */
    + 4 /* ItemCategory */ + item.Properties.Size;
}
