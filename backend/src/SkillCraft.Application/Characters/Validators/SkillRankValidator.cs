using FluentValidation;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Validators;

internal class SkillRankValidator : AbstractValidator<SkillRankModel>
{
  public SkillRankValidator()
  {
    RuleFor(x => x.Skill).IsInEnum();
    RuleFor(x => x.Rank).GreaterThanOrEqualTo(0);
  }
}
