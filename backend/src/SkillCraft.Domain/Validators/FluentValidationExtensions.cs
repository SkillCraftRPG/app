using FluentValidation;

namespace SkillCraft.Domain.Validators;

public static class FluentValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(LocaleUnit.MaximumLength)
      .SetValidator(new LocaleValidator<T>());
  }
}
