using FluentValidation;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Domain.Lineages.Validators;

public class AgesValidator : AbstractValidator<IAges>
{
  public AgesValidator()
  {
    When(x => x.Adolescent.HasValue, () => RuleFor(x => x.Adolescent!.Value).GreaterThan(0));
    When(x => x.Adult.HasValue && x.Adolescent.HasValue, () => RuleFor(x => x.Adult!.Value).GreaterThan(x => x.Adolescent!.Value));
    When(x => x.Mature.HasValue && x.Adult.HasValue, () => RuleFor(x => x.Mature!.Value).GreaterThan(x => x.Adult!.Value));
    When(x => x.Venerable.HasValue && x.Mature.HasValue, () => RuleFor(x => x.Venerable!.Value).GreaterThan(x => x.Mature!.Value));
  }
}
