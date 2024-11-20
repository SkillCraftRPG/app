using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Natures.Queries;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.GraphQL.Natures;

internal static class NatureQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<NatureGraphType>("nature")
      .Authorize()
      .Description("Retrieves a single nature.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the nature.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadNatureQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<NatureSearchResultsGraphType>>("natures")
      .Authorize()
      .Description("Searches a list of natures.")
      .Argument<NonNullGraphType<SearchNaturesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchNaturesQuery(
        context.GetArgument<SearchNaturesPayload>("payload")
      ), context.CancellationToken));
  }
}
