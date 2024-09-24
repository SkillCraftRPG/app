using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Comments.Validators;

internal class TextValidator : AbstractValidator<Text>
{
  public TextValidator()
  {
    RuleFor(x => x.Value).CommentText();
  }
}
