using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MoneyProperties : PropertiesBase, IMoneyProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Money;

  [JsonIgnore]
  public override int Size { get; } = 0;

  [JsonConstructor]
  public MoneyProperties()
  {
  }

  public MoneyProperties(IMoneyProperties _) : this()
  {
  }
}
