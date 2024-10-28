using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MoneyProperties : PropertiesBase, IMoneyProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Money;

  [JsonConstructor]
  public MoneyProperties()
  {
  }

  public MoneyProperties(IMoneyProperties _) : this()
  {
  }
}
