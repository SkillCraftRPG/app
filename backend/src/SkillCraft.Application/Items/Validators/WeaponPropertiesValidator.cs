using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

public class WeaponPropertiesValidator : AbstractValidator<IWeaponProperties>
{
  public WeaponPropertiesValidator()
  {
    When(x => x.Resistance != null, () => RuleFor(x => x.Resistance).GreaterThan(0));
    RuleForEach(x => x.Traits).IsInEnum();

    RuleForEach(x => x.Damages).SetValidator(new WeaponDamageValidator());
    RuleForEach(x => x.VersatileDamages).SetValidator(new WeaponDamageValidator());

    When(x => x.AmmunitionRange != null, () => RuleFor(x => x.AmmunitionRange!).SetValidator(new WeaponRangeValidator()));
    When(x => x.ThrownRange != null, () => RuleFor(x => x.ThrownRange!).SetValidator(new WeaponRangeValidator()));
    When(x => x.ReloadCount != null, () => RuleFor(x => x.ReloadCount).GreaterThan(1));
  }
}
