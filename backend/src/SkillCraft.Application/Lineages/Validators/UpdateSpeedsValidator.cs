using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateSpeedsValidator : AbstractValidator<UpdateSpeedsPayload>
{
  public UpdateSpeedsValidator()
  {
    When(x => x.Walk.HasValue, () => RuleFor(x => x.Walk!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
    When(x => x.Climb.HasValue, () => RuleFor(x => x.Climb!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
    When(x => x.Swim.HasValue, () => RuleFor(x => x.Swim!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
    When(x => x.Fly.HasValue, () => RuleFor(x => x.Fly!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
    When(x => x.Hover.HasValue, () => RuleFor(x => x.Hover!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
    When(x => x.Burrow.HasValue, () => RuleFor(x => x.Burrow!.Value).InclusiveBetween(0, SpeedsValidator.MaximumSpeed));
  }
}
