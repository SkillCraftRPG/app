using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record ConsumableProperties : PropertiesBase, IConsumableProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Consumable;

  public ConsumableProperties(IConsumableProperties consumable) : this()
  {
  }

  public ConsumableProperties()
  {
    new ConsumablePropertiesValidator().ValidateAndThrow(this);
  }
}
