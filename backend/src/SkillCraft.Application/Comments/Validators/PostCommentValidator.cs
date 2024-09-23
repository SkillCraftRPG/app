using FluentValidation;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Comments.Validators;

internal class PostCommentValidator : AbstractValidator<PostCommentPayload>
{
  public PostCommentValidator()
  {
    RuleFor(x => x.Text).CommentText();
  }
}
