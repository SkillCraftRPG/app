using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Talents.Queries;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.GraphQL.Talents;

internal static class TalentQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<TalentGraphType>("talent")
      .Authorize()
      .Description("Retrieves a single talent.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the talent.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadTalentQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<TalentSearchResultsGraphType>>("talents")
      .Authorize()
      .Description("Searches a list of talents.")
      .Argument<NonNullGraphType<SearchTalentsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchTalentsQuery(
        context.GetArgument<SearchTalentsPayload>("payload")
      ), context.CancellationToken));
  }
}
