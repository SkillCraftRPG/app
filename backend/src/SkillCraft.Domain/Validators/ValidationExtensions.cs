﻿using FluentValidation;

namespace SkillCraft.Domain.Validators;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Name.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Roll<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Roll.MaximumLength)
      .SetValidator(new RollPropertyValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty()
      .MaximumLength(Domain.Slug.MaximumLength)
      .SetValidator(new SlugPropertyValidator<T>());
  }
}
