using GraphQL.Types;
using SkillCraft.Contracts;

namespace SkillCraft.GraphQL;

internal class DamageTypeGraphType : EnumerationGraphType<DamageType>
{
  public DamageTypeGraphType()
  {
    Name = nameof(DamageType);
    Description = "Represents the available damage types.";

    AddValue(DamageType.Acid, string.Empty);
    AddValue(DamageType.Bludgeoning, string.Empty);
    AddValue(DamageType.Cold, string.Empty);
    AddValue(DamageType.Electricity, string.Empty);
    AddValue(DamageType.Fire, string.Empty);
    AddValue(DamageType.Force, string.Empty);
    AddValue(DamageType.Necrotic, string.Empty);
    AddValue(DamageType.Piercing, string.Empty);
    AddValue(DamageType.Poison, string.Empty);
    AddValue(DamageType.Psychic, string.Empty);
    AddValue(DamageType.Radiant, string.Empty);
    AddValue(DamageType.Slashing, string.Empty);
    AddValue(DamageType.Thunder, string.Empty);
  }
  private void AddValue(DamageType value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
