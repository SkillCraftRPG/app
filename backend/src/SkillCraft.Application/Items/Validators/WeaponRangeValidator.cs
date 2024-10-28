using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

internal class WeaponRangeValidator : AbstractValidator<WeaponRangeModel>
{
  public WeaponRangeValidator()
  {
    When(x => x.Normal != null, () => RuleFor(x => x.Normal).GreaterThan(0));
    When(x => x.Long != null, () => RuleFor(x => x.Long).GreaterThan(0));
    When(x => x.Normal != null && x.Long != null, () =>
    {
      RuleFor(x => x.Normal).LessThan(x => x.Long);
      RuleFor(x => x.Long).GreaterThan(x => x.Normal);
    });
  }
}
