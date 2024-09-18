using GraphQL.Types;
using SkillCraft.Contracts.Personalities;
using SkillCraft.GraphQL.Customizations;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Personalities;

internal class PersonalityGraphType : AggregateGraphType<PersonalityModel>
{
  public PersonalityGraphType() : base("Represents a character personality.")
  {
    Field(x => x.Name)
      .Description("The display name of the personality.");
    Field(x => x.Description)
      .Description("The description of the personality.");

    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("The attribute to which this personality grants a bonus.");
    Field(x => x.Gift, type: typeof(CustomizationGraphType))
      .Description("The gift granted to characters with this personality.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the personality resides.");
  }
}
