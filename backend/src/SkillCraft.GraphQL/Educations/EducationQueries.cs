using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Educations.Queries;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.GraphQL.Educations;

internal static class EducationQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<EducationGraphType>("education")
      .Authorize()
      .Description("Retrieves a single education.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the education.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadEducationQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<EducationSearchResultsGraphType>>("educations")
      .Authorize()
      .Description("Searches a list of educations.")
      .Argument<NonNullGraphType<SearchEducationsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchEducationsQuery(
        context.GetArgument<SearchEducationsPayload>("payload")
      ), context.CancellationToken));
  }
}
