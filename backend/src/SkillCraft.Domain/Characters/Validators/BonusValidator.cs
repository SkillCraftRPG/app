using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Characters.Validators;

internal class BonusValidator : AbstractValidator<Bonus>
{
  public BonusValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    RuleFor(x => x.Target).BonusTarget();
    RuleFor(x => x.Value).NotEqual(0);
  }
}
