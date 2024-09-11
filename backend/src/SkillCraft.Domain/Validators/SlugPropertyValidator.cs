using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Domain.Validators;

internal class SlugPropertyValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "SlugValidator";

  public string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).";

  public bool IsValid(ValidationContext<T> context, string value) => value.Split('-').All(word => word.All(char.IsLetterOrDigit));
}
