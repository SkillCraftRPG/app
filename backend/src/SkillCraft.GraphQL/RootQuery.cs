using GraphQL.Types;
using SkillCraft.GraphQL.Aspects;
using SkillCraft.GraphQL.Castes;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    AspectQueries.Register(this);
    CasteQueries.Register(this);
    WorldQueries.Register(this);
  }
}
