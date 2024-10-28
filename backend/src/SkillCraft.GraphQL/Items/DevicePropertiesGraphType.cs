using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class DevicePropertiesGraphType : ObjectGraphType<DevicePropertiesModel>
{
  public DevicePropertiesGraphType()
  {
    Name = nameof(DevicePropertiesModel).Remove("Model");
    Description = "Represents the properties of device items.";
  }
}
