using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.GraphQL.Languages;

internal static class LanguageQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<LanguageGraphType>("language")
      .Authorize()
      .Description("Retrieves a single language.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the language.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadLanguageQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<LanguageSearchResultsGraphType>>("languages")
      .Authorize()
      .Description("Searches a list of languages.")
      .Argument<NonNullGraphType<SearchLanguagesPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchLanguagesQuery(
        context.GetArgument<SearchLanguagesPayload>("payload")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<ScriptSearchResultsGraphType>>("scripts")
      .Authorize()
      .Description("TODO")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchScriptsQuery(), context.CancellationToken));
  }
}
