using GraphQL.Types;
using SkillCraft.Contracts.Languages;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Languages;

internal class LanguageGraphType : AggregateGraphType<LanguageModel>
{
  public LanguageGraphType() : base("Represents a character language.")
  {
    Field(x => x.Name)
      .Description("The display name of the language.");
    Field(x => x.Description)
      .Description("The description of the language.");

    Field(x => x.Script)
      .Description("The main script used to write the language. Some languages have no written form.");
    Field(x => x.TypicalSpeakers)
      .Description("The typical speakers of the language. Extinct languages have no speaker left.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the language resides.");
  }
}
