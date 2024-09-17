using GraphQL.Types;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.GraphQL.Aspects;

internal class SkillsGraphType : ObjectGraphType<SkillsModel>
{
  public SkillsGraphType()
  {
    Name = "Skills";
    Description = "Represents the skill selection of an aspect.";

    Field(x => x.Discounted1, type: typeof(SkillGraphType))
      .Description("The first discounted skill talent of the aspect.");
    Field(x => x.Discounted2, type: typeof(SkillGraphType))
      .Description("The second discounted skill talent of the aspect.");
  }
}
