using GraphQL.Types;
using SkillCraft.Contracts.Parties;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Parties;

internal class PartyGraphType : AggregateGraphType<PartyModel>
{
  public PartyGraphType() : base("Represents a character party.")
  {
    Field(x => x.Name)
      .Description("The display name of the party.");
    Field(x => x.Description)
      .Description("The description of the party.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the party resides.");
  }
}
