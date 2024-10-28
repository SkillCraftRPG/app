using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MiscellaneousProperties : PropertiesBase, IMiscellaneousProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Miscellaneous;

  [JsonConstructor]
  public MiscellaneousProperties()
  {
  }

  public MiscellaneousProperties(IMiscellaneousProperties _) : this()
  {
  }
}
