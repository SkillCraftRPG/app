using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Items;

internal static class PropertiesExtensions
{
  public static ConsumableProperties ToConsumableProperties(this IConsumableProperties properties, WorldId worldId)
  {
    ItemId? replaceWithItemWhenEmptyId = null;
    if (properties.ReplaceWithItemWhenEmptyId.HasValue)
    {
      replaceWithItemWhenEmptyId = new(worldId, properties.ReplaceWithItemWhenEmptyId.Value);
    }
    return new ConsumableProperties(properties.Charges, properties.RemoveWhenEmpty, replaceWithItemWhenEmptyId);
  }

  public static WeaponProperties ToWeaponProperties(this IWeaponProperties properties)
  {
    return new WeaponProperties(
      properties.Attack,
      properties.Resistance,
      properties.Traits,
      properties.Damages.Select(damage => new WeaponDamage(new Roll(damage.Roll), damage.Type)).ToArray(),
      properties.VersatileDamages.Select(damage => new WeaponDamage(new Roll(damage.Roll), damage.Type)).ToArray(),
      properties.Range == null ? null : new WeaponRange(properties.Range.Normal, properties.Range.Long),
      properties.ReloadCount);
  }
}
