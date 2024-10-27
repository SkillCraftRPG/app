using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public partial record WeaponProperties : PropertiesBase, IWeaponProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Weapon;

  public WeaponProperties(IWeaponProperties weapon) : this()
  {
  }

  public WeaponProperties()
  {
    new WeaponPropertiesValidator().ValidateAndThrow(this);
  }
}
