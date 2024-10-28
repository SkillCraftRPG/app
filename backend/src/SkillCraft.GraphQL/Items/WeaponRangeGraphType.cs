using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class WeaponRangeGraphType : ObjectGraphType<WeaponRangeModel>
{
  public WeaponRangeGraphType()
  {
    Name = nameof(WeaponRangeModel).Remove("Model");
    Description = "Represents the range of a weapon.";

    Field(x => x.Normal)
      .Description("The normal range of the weapon, in squares (1 square = 1.5 meters = 5 feet).");
    Field(x => x.Long)
      .Description("The long range of the weapon, which inflicts disadvantage to the attack check, in squares (1 square = 1.5 meters = 5 feet).");
  }
}
