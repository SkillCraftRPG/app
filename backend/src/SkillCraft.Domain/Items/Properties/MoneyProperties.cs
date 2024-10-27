using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MoneyProperties : PropertiesBase, IMoneyProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Money;

  public MoneyProperties(IMoneyProperties money) : this()
  {
  }

  public MoneyProperties()
  {
    new MoneyPropertiesValidator().ValidateAndThrow(this);
  }
}
