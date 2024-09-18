using FluentValidation;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateAgesValidator : AbstractValidator<UpdateAgesPayload>
{
  public UpdateAgesValidator()
  {
    When(x => x.Adolescent?.Value != null, () => RuleFor(x => x.Adolescent!.Value).GreaterThan(0));
    When(x => x.Adult?.Value != null, () => RuleFor(x => x.Adult!.Value).GreaterThan(0));
    When(x => x.Mature?.Value != null, () => RuleFor(x => x.Mature!.Value).GreaterThan(0));
    When(x => x.Venerable?.Value != null, () => RuleFor(x => x.Venerable!.Value).GreaterThan(0));
  }
}
