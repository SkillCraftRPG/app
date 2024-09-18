using GraphQL.Types;
using SkillCraft.Contracts.Customizations;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Customizations;

internal class CustomizationGraphType : AggregateGraphType<CustomizationModel>
{
  public CustomizationGraphType() : base("Represents a character customization.")
  {
    Field(x => x.Type, type: typeof(NonNullGraphType<CustomizationTypeGraphType>))
      .Description("The type of the customization.");

    Field(x => x.Name)
      .Description("The display name of the customization.");
    Field(x => x.Description)
      .Description("The description of the customization.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the customization resides.");
  }
}
