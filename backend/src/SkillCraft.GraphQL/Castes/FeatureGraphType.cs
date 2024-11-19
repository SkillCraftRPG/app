using GraphQL.Types;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.GraphQL.Castes;

internal class FeatureGraphType : ObjectGraphType<FeatureModel>
{
  public FeatureGraphType()
  {
    Name = "Feature";
    Description = "Represents a feature granted by a caste.";

    Field(x => x.Id)
      .Description("The unique identifier of the feature.");

    Field(x => x.Name)
      .Description("The display name of the feature.");
    Field(x => x.Description)
      .Description("The description of the feature.");
  }
}
