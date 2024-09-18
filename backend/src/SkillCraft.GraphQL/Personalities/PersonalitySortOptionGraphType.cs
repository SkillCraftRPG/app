using GraphQL.Types;
using SkillCraft.Contracts.Personalities;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Personalities;

internal class PersonalitySortOptionGraphType : SortOptionInputGraphType<PersonalitySortOption>
{
  public PersonalitySortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<PersonalitySortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
