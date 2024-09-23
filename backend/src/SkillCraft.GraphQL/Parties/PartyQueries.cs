using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Parties.Queries;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.GraphQL.Parties;

internal static class PartyQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<PartyGraphType>("party")
      .Authorize()
      .Description("Retrieves a single party.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the party.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadPartyQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<PartySearchResultsGraphType>>("parties")
      .Authorize()
      .Description("Searches a list of parties.")
      .Argument<NonNullGraphType<SearchPartiesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchPartiesQuery(
        context.GetArgument<SearchPartiesPayload>("payload")
      ), context.CancellationToken));
  }
}
