using GraphQL.Types;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.GraphQL;

internal class AttributeGraphType : EnumerationGraphType<Attribute>
{
  public AttributeGraphType()
  {
    Name = nameof(Attribute);
    Description = "Represents the available character attributes.";

    AddValue(Attribute.Agility, string.Empty);
    AddValue(Attribute.Coordination, string.Empty);
    AddValue(Attribute.Intellect, string.Empty);
    AddValue(Attribute.Presence, string.Empty);
    AddValue(Attribute.Sensitivity, string.Empty);
    AddValue(Attribute.Spirit, string.Empty);
    AddValue(Attribute.Vigor, string.Empty);
  }
  private void AddValue(Attribute value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
