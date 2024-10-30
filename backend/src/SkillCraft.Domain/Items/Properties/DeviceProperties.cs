using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record DeviceProperties : PropertiesBase, IDeviceProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Device;

  [JsonIgnore]
  public override int Size { get; } = 0;

  [JsonConstructor]
  public DeviceProperties()
  {
  }

  public DeviceProperties(IDeviceProperties _) : this()
  {
  }
}
