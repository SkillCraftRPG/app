using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class CharacterLanguageValidator : AbstractValidator<CharacterLanguagePayload>
{
  public CharacterLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Description());
  }
}
