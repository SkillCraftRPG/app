using FluentValidation;
using SkillCraft.Contracts;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Size
{
  public SizeCategory Category { get; }
  public Roll? Roll { get; }

  public Size() : this(SizeCategory.Medium, roll: null)
  {
  }

  [JsonConstructor]
  public Size(SizeCategory category, Roll? roll)
  {
    Category = category;
    Roll = roll;
    new SizeValidator().ValidateAndThrow(this);
  }
}
