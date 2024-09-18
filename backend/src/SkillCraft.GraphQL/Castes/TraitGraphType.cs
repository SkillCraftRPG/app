using GraphQL.Types;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.GraphQL.Castes;

internal class TraitGraphType : ObjectGraphType<TraitModel>
{
  public TraitGraphType()
  {
    Name = "Trait";
    Description = "Represents a trait granted by a caste.";

    Field(x => x.Id)
      .Description("The unique identifier of the trait.");

    Field(x => x.Name)
      .Description("The display name of the trait.");
    Field(x => x.Description)
      .Description("The description of the trait.");
  }
}
