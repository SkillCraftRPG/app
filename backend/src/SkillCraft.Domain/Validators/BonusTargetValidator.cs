using FluentValidation;
using FluentValidation.Validators;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Validators;

internal class BonusTargetValidator<T> : IPropertyValidator<T, string> where T : IBonus
{
  public string Name { get; } = "BonusTargetValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' is not a valid bonus target to the specified bonus category.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    BonusCategory category = context.InstanceToValidate.Category;
    return category switch
    {
      BonusCategory.Attribute => Enum.TryParse(value, out Attribute attribute) && Enum.IsDefined(attribute),
      BonusCategory.Miscellaneous => Enum.TryParse(value, out MiscellaneousBonusTarget miscellaneous) && Enum.IsDefined(miscellaneous),
      BonusCategory.Skill => Enum.TryParse(value, out Skill skill) && Enum.IsDefined(skill),
      BonusCategory.Speed => Enum.TryParse(value, out SpeedKind speed) && Enum.IsDefined(speed),
      BonusCategory.Statistic => Enum.TryParse(value, out Statistic statistic) && Enum.IsDefined(statistic),
      _ => true,
    };
  }
}
