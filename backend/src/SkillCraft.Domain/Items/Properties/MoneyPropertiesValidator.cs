using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class MoneyPropertiesValidator : AbstractValidator<IMoneyProperties>
{
  public MoneyPropertiesValidator()
  {
  }
}
