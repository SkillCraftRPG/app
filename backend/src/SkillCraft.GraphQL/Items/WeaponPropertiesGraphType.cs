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

    Field(x => x.Attack)
      .Description("The bonus to attack checks granted by the weapon.");
    Field(x => x.Resistance)
      .Description("The resistance points of the weapon.");
    Field(x => x.Traits, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<WeaponTraitGraphType>>>))
      .Description("The traits of the weapon.");

    Field(x => x.Damages, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<WeaponDamageGraphType>>>))
      .Description("The damage inflicted by the weapon.");
    Field(x => x.VersatileDamages, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<WeaponDamageGraphType>>>))
      .Description("The damage inflicted by the versatile weapon when held two-handed.");

    Field(x => x.Range, type: typeof(WeaponRangeGraphType))
      .Description("The range values of the weapon.");
    Field(x => x.ReloadCount)
      .Description("The number of ammunition piece that can be fired from the weapon before a full-reload is required.");
  }
}
