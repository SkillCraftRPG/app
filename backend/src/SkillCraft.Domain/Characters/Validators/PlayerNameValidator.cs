using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Characters.Validators;

internal class PlayerNameValidator : AbstractValidator<PlayerName>
{
  public PlayerNameValidator()
  {
    RuleFor(x => x.Value).Name();
  }
}
