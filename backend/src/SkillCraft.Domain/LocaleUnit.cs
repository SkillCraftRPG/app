using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record LocaleUnit
{
  public const int MaximumLength = 16;

  public string Code { get; }
  public CultureInfo Culture { get; }

  public LocaleUnit(string code)
  {
    Code = code.Trim();
    new LocaleUnitValidator().ValidateAndThrow(this);

    Culture = CultureInfo.GetCultureInfo(Code);
    Code = Culture.Name;
  }
}
