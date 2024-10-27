using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class MiscellaneousPropertiesValidator : AbstractValidator<IMiscellaneousProperties>
{
  public MiscellaneousPropertiesValidator()
  {
  }
}
