using FluentValidation;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Validators;

internal class LevelUpCharacterValidator : AbstractValidator<LevelUpCharacterPayload>
{
  public LevelUpCharacterValidator()
  {
    RuleFor(x => x.Attribute).IsInEnum();
  }
}
