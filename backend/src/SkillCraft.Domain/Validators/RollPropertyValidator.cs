using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Domain.Validators;

internal class RollPropertyValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "RollValidator";

  public string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' must be composed of two positive numbers separated by a 'D' (lowercase or uppercase).";

  public bool IsValid(ValidationContext<T> context, string value)
  {
    string[] values = value.ToLowerInvariant().Split('d');
    return values.Length == 2 && values.All(value => byte.TryParse(value, out byte number) && number > 0);
  }
}
