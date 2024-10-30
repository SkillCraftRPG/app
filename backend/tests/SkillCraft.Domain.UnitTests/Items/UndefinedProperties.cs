using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Domain.Items;

internal record UndefinedProperties : PropertiesBase
{
  public override ItemCategory Category { get; } = (ItemCategory)(-1);
  public override int Size { get; } = 0;
}
