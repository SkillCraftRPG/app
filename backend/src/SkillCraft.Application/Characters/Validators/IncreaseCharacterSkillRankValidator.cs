using FluentValidation;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Validators;

internal class IncreaseCharacterSkillRankValidator : AbstractValidator<IncreaseCharacterSkillRankPayload>
{
  public IncreaseCharacterSkillRankValidator()
  {
    RuleFor(x => x.Skill).IsInEnum();
  }
}
