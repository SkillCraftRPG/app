using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Castes.Queries;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.GraphQL.Castes;

internal static class CasteQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<CasteGraphType>("caste")
      .Authorize()
      .Description("Retrieves a single caste.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the caste.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadCasteQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<CasteSearchResultsGraphType>>("castes")
      .Authorize()
      .Description("Searches a list of castes.")
      .Argument<NonNullGraphType<SearchCastesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchCastesQuery(
        context.GetArgument<SearchCastesPayload>("payload")
      ), context.CancellationToken));
  }
}
