using GraphQL.Types;

namespace SkillCraft.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);
  }
}
