using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MiscellaneousProperties : PropertiesBase, IMiscellaneousProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Miscellaneous;

  [JsonIgnore]
  public override int Size { get; } = 0;

  [JsonConstructor]
  public MiscellaneousProperties()
  {
  }

  public MiscellaneousProperties(IMiscellaneousProperties _) : this()
  {
  }
}
