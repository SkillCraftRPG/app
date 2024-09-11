using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class DescriptionValidator : AbstractValidator<Description>
{
  public DescriptionValidator()
  {
    RuleFor(x => x.Value).Description();
  }
}
