using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Customizations.Queries;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.GraphQL.Customizations;

internal static class CustomizationQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<CustomizationGraphType>("customization")
      .Authorize()
      .Description("Retrieves a single customization.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the customization.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadCustomizationQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<CustomizationSearchResultsGraphType>>("customizations")
      .Authorize()
      .Description("Searches a list of customizations.")
      .Argument<NonNullGraphType<SearchCustomizationsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchCustomizationsQuery(
        context.GetArgument<SearchCustomizationsPayload>("payload")
      ), context.CancellationToken));
  }
}
