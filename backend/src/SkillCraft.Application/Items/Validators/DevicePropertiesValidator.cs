using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

internal class DevicePropertiesValidator : AbstractValidator<IDeviceProperties>
{
  public DevicePropertiesValidator()
  {
  }
}
