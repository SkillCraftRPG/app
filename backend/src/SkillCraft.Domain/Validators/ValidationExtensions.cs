using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(SlugUnit.MaximumLength).SetValidator(new SlugValidator<T>());
  }
}
