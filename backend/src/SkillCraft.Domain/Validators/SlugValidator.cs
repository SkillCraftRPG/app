using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Domain.Validators;

internal class SlugValidator<T> : PropertyValidator<T, string>
{
  public override string Name { get; } = "SlugValidator";

  public override bool IsValid(ValidationContext<T> context, string value)
  {
    string[] terms = value.Split('-');
    return terms.All(term => !string.IsNullOrEmpty(term) && term.All(c => char.IsLetterOrDigit(c)));
  }

  protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).";
}
