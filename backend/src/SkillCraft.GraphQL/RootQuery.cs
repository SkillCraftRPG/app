using GraphQL.Types;
using SkillCraft.GraphQL.Aspects;
using SkillCraft.GraphQL.Castes;
using SkillCraft.GraphQL.Comments;
using SkillCraft.GraphQL.Customizations;
using SkillCraft.GraphQL.Educations;
using SkillCraft.GraphQL.Items;
using SkillCraft.GraphQL.Languages;
using SkillCraft.GraphQL.Lineages;
using SkillCraft.GraphQL.Parties;
using SkillCraft.GraphQL.Personalities;
using SkillCraft.GraphQL.Talents;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    AspectQueries.Register(this);
    CasteQueries.Register(this);
    CommentQueries.Register(this);
    CustomizationQueries.Register(this);
    EducationQueries.Register(this);
    ItemQueries.Register(this);
    LanguageQueries.Register(this);
    LineageQueries.Register(this);
    PartyQueries.Register(this);
    PersonalityQueries.Register(this);
    TalentQueries.Register(this);
    WorldQueries.Register(this);
  }
}
