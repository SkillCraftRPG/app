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
    When(x => x.VersatileDamages.Count > 0, () => RuleFor(x => x.Traits).Must(traits => traits.Contains(WeaponTrait.Versatile))
      .WithErrorCode(nameof(WeaponPropertiesValidator))
      .WithMessage($"'{{PropertyName}}' must include '{WeaponTrait.Versatile}' when versatile damages are specified."));

    When(x => x.Range != null, () =>
    {
      RuleFor(x => x.Range!).SetValidator(new WeaponRangeValidator());
      RuleFor(x => x.Traits).Must(traits => traits.Contains(WeaponTrait.Range))
        .WithErrorCode(nameof(WeaponPropertiesValidator))
        .WithMessage($"'{{PropertyName}}' must include '{WeaponTrait.Range}' when a range is specified.");
    });
    When(x => x.ReloadCount != null, () =>
    {
      RuleFor(x => x.ReloadCount).GreaterThan(1);
      RuleFor(x => x.Traits).Must(traits => traits.Contains(WeaponTrait.Reload))
        .WithErrorCode(nameof(WeaponPropertiesValidator))
        .WithMessage($"'{{PropertyName}}' must include '{WeaponTrait.Reload}' when a reload count is specified.");
    });
  }
}
