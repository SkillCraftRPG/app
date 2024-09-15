using FluentValidation;
using SkillCraft.Domain.Languages.Validators;

namespace SkillCraft.Domain.Languages;

public record TypicalSpeakers
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public TypicalSpeakers(string value)
  {
    Value = value.Trim();
    new TypicalSpeakersValidator().ValidateAndThrow(this);
  }

  public static TypicalSpeakers? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
