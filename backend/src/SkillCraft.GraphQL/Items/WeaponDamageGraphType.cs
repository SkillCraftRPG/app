using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class WeaponDamageGraphType : ObjectGraphType<WeaponDamageModel>
{
  public WeaponDamageGraphType()
  {
    Name = nameof(WeaponDamageModel).Remove("Model");
    Description = "Represents a damage roll.";

    Field(x => x.Roll)
      .Description("The roll of the damage.");
    Field(x => x.Type, type: typeof(NonNullGraphType<DamageTypeGraphType>))
      .Description("The type of the damage.");
  }
}
