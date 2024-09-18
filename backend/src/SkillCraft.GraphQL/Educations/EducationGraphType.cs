using GraphQL.Types;
using SkillCraft.Contracts.Educations;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Educations;

internal class EducationGraphType : AggregateGraphType<EducationModel>
{
  public EducationGraphType() : base("Represents a character education.")
  {
    Field(x => x.Name)
      .Description("The display name of the education.");
    Field(x => x.Description)
      .Description("The description of the education.");

    Field(x => x.Skill, type: typeof(SkillGraphType))
      .Description("The skill talent required by this education.");
    Field(x => x.WealthMultiplier)
      .Description("The starting wealth multiplier of characters having this education.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the education resides.");
  }
}
