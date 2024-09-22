using FluentValidation;
using SkillCraft.Domain.Characters.Validators;

namespace SkillCraft.Domain.Characters;

public record PlayerName
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public PlayerName(string value)
  {
    Value = value.Trim();
    new PlayerNameValidator().ValidateAndThrow(this);
  }

  public static PlayerName? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
