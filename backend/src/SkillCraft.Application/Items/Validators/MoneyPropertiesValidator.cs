using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

internal class MoneyPropertiesValidator : AbstractValidator<IMoneyProperties>
{
  public MoneyPropertiesValidator()
  {
  }
}
