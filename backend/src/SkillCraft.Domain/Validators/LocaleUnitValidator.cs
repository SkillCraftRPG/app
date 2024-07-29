using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class LocaleUnitValidator : AbstractValidator<LocaleUnit>
{
  public LocaleUnitValidator()
  {
    RuleFor(x => x.Code).Locale();
  }
}
