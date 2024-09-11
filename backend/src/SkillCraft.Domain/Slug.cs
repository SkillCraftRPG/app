using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record Slug
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Slug(string value)
  {
    Value = value.Trim();
    new SlugValidator().ValidateAndThrow(this);
  }

  public static Slug? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
