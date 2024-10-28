using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class WeaponPropertiesGraphType : ObjectGraphType<WeaponPropertiesModel>
{
  public WeaponPropertiesGraphType()
  {
    Name = nameof(WeaponPropertiesModel).Remove("Model");
    Description = "Represents the properties of weapon items.";
  }
}
