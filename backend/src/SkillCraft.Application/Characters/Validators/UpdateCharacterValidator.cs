using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class UpdateCharacterValidator : AbstractValidator<UpdateCharacterPayload>
{
  public UpdateCharacterValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Player?.Value), () => RuleFor(x => x.Player!.Value!).Name());

    RuleFor(x => x.Height).GreaterThan(0.0);
    RuleFor(x => x.Weight).GreaterThan(0.0);
    RuleFor(x => x.Age).GreaterThan(0);

    RuleFor(x => x.Experience).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Vitality).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Stamina).GreaterThanOrEqualTo(0);
    RuleFor(x => x.BloodAlcoholContent).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Intoxication).GreaterThanOrEqualTo(0);
  }
}
