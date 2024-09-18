using GraphQL.Types;
using SkillCraft.GraphQL.Aspects;
using SkillCraft.GraphQL.Castes;
using SkillCraft.GraphQL.Customizations;
using SkillCraft.GraphQL.Educations;
using SkillCraft.GraphQL.Personalities;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    AspectQueries.Register(this);
    CasteQueries.Register(this);
    CustomizationQueries.Register(this);
    EducationQueries.Register(this);
    PersonalityQueries.Register(this);
    WorldQueries.Register(this);
  }
}
