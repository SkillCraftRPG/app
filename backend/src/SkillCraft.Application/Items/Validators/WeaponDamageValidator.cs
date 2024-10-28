using FluentValidation;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Items.Validators;

internal class WeaponDamageValidator : AbstractValidator<WeaponDamageModel>
{
  public WeaponDamageValidator()
  {
    RuleFor(x => x.Roll).Roll();
    RuleFor(x => x.Type).IsInEnum();
  }
}
