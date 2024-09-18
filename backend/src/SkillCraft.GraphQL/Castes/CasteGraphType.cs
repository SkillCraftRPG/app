using GraphQL.Types;
using SkillCraft.Contracts.Castes;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Castes;

internal class CasteGraphType : AggregateGraphType<CasteModel>
{
  public CasteGraphType() : base("Represents a character caste.")
  {
    Field(x => x.Name)
      .Description("The display name of the caste.");
    Field(x => x.Description)
      .Description("The description of the caste.");

    Field(x => x.Skill, type: typeof(SkillGraphType))
      .Description("The skill talent required by this caste.");
    Field(x => x.WealthRoll)
      .Description("The starting wealth roll of characters in this caste.");

    Field(x => x.Traits, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<TraitGraphType>>>))
      .Description("The traits granted to characters in this caste.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the caste resides.");
  }
}
