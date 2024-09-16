using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Worlds.Queries;

namespace SkillCraft.GraphQL.Worlds;

internal static class WorldQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<WorldGraphType>("world")
      .Authorize()
      .Description("Retrieves a single world.")
      .Argument<IdGraphType>(name: "id", description: "The unique identifier of the world.")
      .Argument<StringGraphType>(name: "slug", description: "The unique slug of the world.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadWorldQuery(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("slug")
      ), context.CancellationToken));
  }
}
