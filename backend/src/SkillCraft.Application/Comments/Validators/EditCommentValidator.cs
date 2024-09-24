using FluentValidation;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Comments.Validators;

internal class EditCommentValidator : AbstractValidator<EditCommentPayload>
{
  public EditCommentValidator()
  {
    RuleFor(x => x.Text).CommentText();
  }
}
