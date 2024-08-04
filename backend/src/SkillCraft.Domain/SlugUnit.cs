using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record SlugUnit
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public SlugUnit(string value)
  {
    Value = value.Trim();
    new SlugUnitValidator().ValidateAndThrow(this);
  }

  public static SlugUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
}
