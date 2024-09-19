using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Lineages.Queries;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.GraphQL.Lineages;

internal static class LineageQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<LineageGraphType>("lineage")
      .Authorize()
      .Description("Retrieves a single lineage.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the lineage.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadLineageQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<LineageSearchResultsGraphType>>("lineages")
      .Authorize()
      .Description("Searches a list of lineages.")
      .Argument<NonNullGraphType<SearchLineagesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchLineagesQuery(
        context.GetArgument<SearchLineagesPayload>("payload")
      ), context.CancellationToken));
  }
}
