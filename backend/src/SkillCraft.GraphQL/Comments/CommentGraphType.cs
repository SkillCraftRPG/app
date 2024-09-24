using GraphQL.Types;
using SkillCraft.Contracts.Comments;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Comments;

internal class CommentGraphType : AggregateGraphType<CommentModel>
{
  public CommentGraphType() : base("Represents a character comment.")
  {
    Field(x => x.Text)
      .Description("The text contents of the comment.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the comment resides.");
  }
}
