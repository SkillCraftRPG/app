using FluentValidation;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Domain.Lineages.Validators;

public class SpeedsValidator : AbstractValidator<ISpeeds>
{
  public const int MaximumSpeed = 8;

  public SpeedsValidator()
  {
    RuleFor(x => x.Walk).InclusiveBetween(0, MaximumSpeed);
    RuleFor(x => x.Climb).InclusiveBetween(0, MaximumSpeed);
    RuleFor(x => x.Swim).InclusiveBetween(0, MaximumSpeed);
    RuleFor(x => x.Fly).InclusiveBetween(0, MaximumSpeed);
    RuleFor(x => x.Hover).InclusiveBetween(0, MaximumSpeed);
    RuleFor(x => x.Burrow).InclusiveBetween(0, MaximumSpeed);
  }
}
