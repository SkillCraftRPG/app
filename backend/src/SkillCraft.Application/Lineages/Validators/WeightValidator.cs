using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class WeightValidator : AbstractValidator<WeightModel>
{
  public WeightValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Starved), () => RuleFor(x => x.Starved!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Skinny), () => RuleFor(x => x.Skinny!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Normal), () => RuleFor(x => x.Normal!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Overweight), () => RuleFor(x => x.Overweight!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Obese), () => RuleFor(x => x.Obese!).Roll());
  }
}
