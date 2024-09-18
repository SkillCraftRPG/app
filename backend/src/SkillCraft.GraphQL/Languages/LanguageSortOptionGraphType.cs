using GraphQL.Types;
using SkillCraft.Contracts.Languages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Languages;

internal class LanguageSortOptionGraphType : SortOptionInputGraphType<LanguageSortOption>
{
  public LanguageSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<LanguageSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
