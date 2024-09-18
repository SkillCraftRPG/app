using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class WeightValidator : AbstractValidator<WeightModel>
{
  public WeightValidator()
  {
    RuleFor(x => x.Starved).Roll();
    RuleFor(x => x.Skinny).Roll();
    RuleFor(x => x.Normal).Roll();
    RuleFor(x => x.Overweight).Roll();
    RuleFor(x => x.Obese).Roll();
  }
}
