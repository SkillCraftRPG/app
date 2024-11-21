using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class BonusValidator : AbstractValidator<BonusPayload>
{
  public BonusValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    RuleFor(x => x.Target).BonusTarget();
    RuleFor(x => x.Value).NotEqual(0);

    When(x => !string.IsNullOrWhiteSpace(x.Precision), () => RuleFor(x => x.Precision!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Description());
  }
}
