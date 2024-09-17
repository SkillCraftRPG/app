using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Aspects.Queries;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.GraphQL.Aspects;

internal static class AspectQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<AspectGraphType>("aspect")
      .Authorize()
      .Description("Retrieves a single aspect.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the aspect.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadAspectQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<AspectSearchResultsGraphType>>("aspects")
      .Authorize()
      .Description("Searches a list of aspects.")
      .Argument<NonNullGraphType<SearchAspectsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchAspectsQuery(
        context.GetArgument<SearchAspectsPayload>("payload")
      ), context.CancellationToken));
  }
}
