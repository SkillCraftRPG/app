using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record Roll
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public Roll(string value)
  {
    Value = value.Trim();
    new RollValidator().ValidateAndThrow(this);
  }

  public static Roll? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
