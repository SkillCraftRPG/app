using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

internal class MiscellaneousPropertiesValidator : AbstractValidator<IMiscellaneousProperties>
{
  public MiscellaneousPropertiesValidator()
  {
  }
}
