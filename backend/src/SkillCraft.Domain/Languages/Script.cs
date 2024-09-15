using FluentValidation;
using SkillCraft.Domain.Languages.Validators;

namespace SkillCraft.Domain.Languages;

public record Script
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public Script(string value)
  {
    Value = value.Trim();
    new ScriptValidator().ValidateAndThrow(this);
  }

  public static Script? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
