using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record Name
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Name(string value)
  {
    Value = value.Trim();
    new NameValidator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;
}
