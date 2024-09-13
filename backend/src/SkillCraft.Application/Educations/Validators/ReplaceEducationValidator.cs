using FluentValidation;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Educations.Validators;

internal class ReplaceEducationValidator : AbstractValidator<ReplaceEducationPayload>
{
  public ReplaceEducationValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Skill.HasValue, () => RuleFor(x => x.Skill!.Value).IsInEnum());
    When(x => x.WealthMultiplier.HasValue, () => RuleFor(x => x.WealthMultiplier!.Value).GreaterThan(0.0));
  }
}
