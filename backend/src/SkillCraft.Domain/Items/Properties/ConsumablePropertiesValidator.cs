using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class ConsumablePropertiesValidator : AbstractValidator<IConsumableProperties>
{
  public ConsumablePropertiesValidator()
  {
  }
}
