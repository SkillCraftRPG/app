using FluentValidation;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Comments.Validators;

internal class CreateCommentValidator : AbstractValidator<CreateCommentPayload>
{
  public CreateCommentValidator()
  {
    RuleFor(x => x.Text).CommentText();
  }
}
