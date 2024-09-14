using FluentValidation;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Domain.Aspects.Validators;

internal class SkillsValidator : AbstractValidator<ISkills>
{
  public SkillsValidator()
  {
    When(x => x.Discounted1.HasValue, () => RuleFor(x => x.Discounted1!.Value).IsInEnum());
    When(x => x.Discounted2.HasValue, () => RuleFor(x => x.Discounted2!.Value).IsInEnum());

    // TODO(fpion): all skills must differ
  }
}
