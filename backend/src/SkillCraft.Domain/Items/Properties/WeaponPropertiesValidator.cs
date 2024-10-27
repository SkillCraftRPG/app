using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class WeaponPropertiesValidator : AbstractValidator<IWeaponProperties>
{
  public WeaponPropertiesValidator()
  {
  }
}
