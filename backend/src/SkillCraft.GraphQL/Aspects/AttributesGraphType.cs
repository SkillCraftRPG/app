using GraphQL.Types;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.GraphQL.Aspects;

internal class AttributesGraphType : ObjectGraphType<AttributesModel>
{
  public AttributesGraphType()
  {
    Name = "Attributes";
    Description = "Represents the attribute selection of an aspect.";

    Field(x => x.Mandatory1, type: typeof(AttributeGraphType))
      .Description("The first mandatory attribute of the aspect.");
    Field(x => x.Mandatory2, type: typeof(AttributeGraphType))
      .Description("The second mandatory attribute of the aspect.");
    Field(x => x.Optional1, type: typeof(AttributeGraphType))
      .Description("The first optional attribute of the aspect.");
    Field(x => x.Optional2, type: typeof(AttributeGraphType))
      .Description("The second optional attribute of the aspect.");
  }
}
