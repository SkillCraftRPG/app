using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Items.Queries;
using SkillCraft.Contracts.Items;

namespace SkillCraft.GraphQL.Items;

internal static class ItemQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<ItemGraphType>("item")
      .Authorize()
      .Description("Retrieves a single item.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the item.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadItemQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<ItemSearchResultsGraphType>>("items")
      .Authorize()
      .Description("Searches a list of items.")
      .Argument<NonNullGraphType<SearchItemsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchItemsQuery(
        context.GetArgument<SearchItemsPayload>("payload")
      ), context.CancellationToken));
  }
}
