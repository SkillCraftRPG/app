using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record DeviceProperties : PropertiesBase, IDeviceProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Device;

  public DeviceProperties(IDeviceProperties device) : this()
  {
  }

  public DeviceProperties()
  {
    new DevicePropertiesValidator().ValidateAndThrow(this);
  }
}
