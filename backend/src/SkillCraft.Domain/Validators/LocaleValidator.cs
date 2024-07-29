using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Domain.Validators;

internal class LocaleValidator<T> : PropertyValidator<T, string>
{
  private const int LOCALE_CUSTOM_UNSPECIFIED = 0x1000;

  public override string Name { get; } = "LocaleValidator";

  public override bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      CultureInfo culture = new(value);
      return !string.IsNullOrEmpty(culture.Name) && culture.LCID != LOCALE_CUSTOM_UNSPECIFIED;
    }
    catch (CultureNotFoundException)
    {
      return false;
    }
  }

  protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' may not be the invariant culture, nor a user-defined culture.";
}
