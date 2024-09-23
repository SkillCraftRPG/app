using GraphQL.Types;
using SkillCraft.Contracts.Talents;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Talents;

internal class TalentGraphType : AggregateGraphType<TalentModel>
{
  public TalentGraphType() : base("Represents a character talent.")
  {
    Field(x => x.Tier)
      .Description("The tier of the talent.");

    Field(x => x.Name)
      .Description("The display name of the talent.");
    Field(x => x.Description)
      .Description("The description of the talent.");

    Field(x => x.AllowMultiplePurchases)
      .Description("A value indicating whether or not the talent can be purchased multiple times.");
    Field(x => x.Skill, type: typeof(SkillGraphType))
      .Description("The skill associated to the talent.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the talent resides.");
  }
}
