using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Personalities.Queries;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.GraphQL.Personalities;

internal static class PersonalityQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<PersonalityGraphType>("personality")
      .Authorize()
      .Description("Retrieves a single personality.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the personality.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadPersonalityQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<PersonalitySearchResultsGraphType>>("personalities")
      .Authorize()
      .Description("Searches a list of personalities.")
      .Argument<NonNullGraphType<SearchPersonalitiesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchPersonalitiesQuery(
        context.GetArgument<SearchPersonalitiesPayload>("payload")
      ), context.CancellationToken));
  }
}
