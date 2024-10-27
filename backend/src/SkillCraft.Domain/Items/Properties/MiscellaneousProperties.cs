using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record MiscellaneousProperties : PropertiesBase, IMiscellaneousProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Miscellaneous;

  public MiscellaneousProperties(IMiscellaneousProperties miscellaneous) : this()
  {
  }

  public MiscellaneousProperties()
  {
    new MiscellaneousPropertiesValidator().ValidateAndThrow(this);
  }
}
