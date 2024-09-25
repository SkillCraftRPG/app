using GraphQL;
using GraphQL.Types;
using SkillCraft.Application.Comments.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Comments;

namespace SkillCraft.GraphQL.Comments;

internal static class CommentQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<CommentGraphType>("comment")
      .Authorize()
      .Description("Retrieves a single comment.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "id", description: "The unique identifier of the comment.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new ReadCommentQuery(
        context.GetArgument<Guid>("id")
      ), context.CancellationToken));

    root.Field<NonNullGraphType<CommentSearchResultsGraphType>>("comments")
      .Authorize()
      .Description("Searches a list of comments.")
      .Argument<NonNullGraphType<EntityTypeGraphType>>(name: "entityType", description: "The type of the containing entity.")
      .Argument<NonNullGraphType<IdGraphType>>(name: "entityId", description: "The unique identifier of the containing entity.")
      .Argument<NonNullGraphType<SearchCommentsPayloadGraphType>>(name: "payload", description: "The parameters to apply to the search.")
      .ResolveAsync(async context => await context.ExecuteQueryAsync(new SearchCommentsQuery(
        context.GetArgument<EntityType>("entityType"),
        context.GetArgument<Guid>("entityId"),
        context.GetArgument<SearchCommentsPayload>("payload")
      ), context.CancellationToken));
  }
}
