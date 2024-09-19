using GraphQL.Types;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.GraphQL.Lineages;

internal class NameCategoryGraphType : ObjectGraphType<NameCategory>
{
  public NameCategoryGraphType()
  {
    Name = "NameCategory";
    Description = "Represents a custom name category.";

    Field(x => x.Key)
      .Description("The unique key of the category.");
    Field(x => x.Values)
      .Description("The list of names in this category.");
  }
}
