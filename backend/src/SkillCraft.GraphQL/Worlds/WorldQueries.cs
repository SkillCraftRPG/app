using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts.Worlds;

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

    root.Field<NonNullGraphType<WorldSearchResultsGraphType>>("worlds")
      .Authorize()
      .Description("Searches a list of worlds.")
      .Argument<NonNullGraphType<SearchWorldsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchWorldsQuery(
        context.GetArgument<SearchWorldsPayload>("payload")
      ), context.CancellationToken));
  }
}
