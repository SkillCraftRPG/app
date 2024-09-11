using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record Description
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Description(string value)
  {
    Value = value.Trim();
    new DescriptionValidator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;
}
