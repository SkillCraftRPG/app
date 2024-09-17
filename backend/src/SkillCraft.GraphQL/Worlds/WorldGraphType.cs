using GraphQL.Types;
using SkillCraft.Contracts.Worlds;
using SkillCraft.GraphQL.Actors;

namespace SkillCraft.GraphQL.Worlds;

internal class WorldGraphType : AggregateGraphType<WorldModel>
{
  public WorldGraphType() : base("Represents a world for the SkillCraft Role-Playing game system. Worlds encompass all game entities.")
  {
    Field(x => x.Slug)
      .Description("The unique slug of the world.");
    Field(x => x.Name)
      .Description("The display name of the world.");
    Field(x => x.Description)
      .Description("The description of the world.");

    Field(x => x.Owner, type: typeof(NonNullGraphType<ActorGraphType>))
      .Description("The owner of the world.");
  }
}
