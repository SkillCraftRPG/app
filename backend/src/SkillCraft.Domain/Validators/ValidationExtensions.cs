using FluentValidation;

namespace SkillCraft.Domain.Validators;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> CommentText<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Comments.Text.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> LanguagesText<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Lineages.Languages.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Name.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> NamesText<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Lineages.Names.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Roll<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Roll.MaximumLength)
      .SetValidator(new RollPropertyValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Script<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Languages.Script.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Slug.MaximumLength)
      .SetValidator(new SlugPropertyValidator<T>());
  }

  public static IRuleBuilderOptions<T, int> Tier<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(0, 3);
  }

  public static IRuleBuilderOptions<T, string> TypicalSpeakers<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Languages.TypicalSpeakers.MaximumLength);
  }
}
