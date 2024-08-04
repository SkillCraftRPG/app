using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class SlugUnitValidator : AbstractValidator<SlugUnit>
{
  public SlugUnitValidator()
  {
    RuleFor(x => x.Value).Slug();
  }
}
