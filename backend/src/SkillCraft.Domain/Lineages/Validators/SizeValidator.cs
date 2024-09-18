using FluentValidation;

namespace SkillCraft.Domain.Lineages.Validators;

internal class SizeValidator : AbstractValidator<Size>
{
  public SizeValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
  }
}
