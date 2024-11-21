using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class CharacterTalentValidator : AbstractValidator<CharacterTalentPayload>
{
  public CharacterTalentValidator()
  {
    RuleFor(x => x.TalentId).NotEmpty();

    RuleFor(x => x.Cost).InclusiveBetween(0, 5);
    When(x => !string.IsNullOrWhiteSpace(x.Precision), () => RuleFor(x => x.Precision!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Description());
  }
}
