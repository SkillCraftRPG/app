using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record Name
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public Name(string value)
  {
    Value = value.Trim();
    new NameValidator().ValidateAndThrow(this);
  }

  public static Name? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
