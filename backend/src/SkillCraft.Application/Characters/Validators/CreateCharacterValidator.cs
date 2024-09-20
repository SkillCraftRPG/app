using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class CreateCharacterValidator : AbstractValidator<CreateCharacterPayload>
{
  public CreateCharacterValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Player), () => RuleFor(x => x.Player!).Name());

    RuleFor(x => x.LineageId).NotEmpty();
    RuleFor(x => x.Height).GreaterThan(0.0);
    RuleFor(x => x.Weight).GreaterThan(0.0);
    RuleFor(x => x.Age).GreaterThan(0);
  }
}
