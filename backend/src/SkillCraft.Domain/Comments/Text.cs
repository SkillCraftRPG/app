using FluentValidation;
using SkillCraft.Domain.Comments.Validators;

namespace SkillCraft.Domain.Comments;

public record Text
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public Text(string value)
  {
    Value = value.Trim();
    new TextValidator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;
}
