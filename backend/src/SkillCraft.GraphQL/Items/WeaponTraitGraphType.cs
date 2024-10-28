using GraphQL.Types;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class WeaponTraitGraphType : EnumerationGraphType<WeaponTrait>
{
  public WeaponTraitGraphType()
  {
    Name = nameof(WeaponTrait);
    Description = "Represents the available weapon traits.";

    AddValue(WeaponTrait.Ammunition, string.Empty);
    AddValue(WeaponTrait.Finesse, string.Empty);
    AddValue(WeaponTrait.Heavy, string.Empty);
    AddValue(WeaponTrait.Light, string.Empty);
    AddValue(WeaponTrait.Loading, string.Empty);
    AddValue(WeaponTrait.Range, string.Empty);
    AddValue(WeaponTrait.Reach, string.Empty);
    AddValue(WeaponTrait.Reload, string.Empty);
    AddValue(WeaponTrait.Scatter, string.Empty);
    AddValue(WeaponTrait.Special, string.Empty);
    AddValue(WeaponTrait.Thrown, string.Empty);
    AddValue(WeaponTrait.TwoHanded, string.Empty);
    AddValue(WeaponTrait.Versatile, string.Empty);
  }
  private void AddValue(WeaponTrait value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
