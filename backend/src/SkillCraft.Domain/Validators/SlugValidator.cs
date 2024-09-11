using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class SlugValidator : AbstractValidator<Slug>
{
  public SlugValidator()
  {
    RuleFor(x => x.Value).Slug();
  }
}
