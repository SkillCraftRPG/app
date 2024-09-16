using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Languages.Validators;

internal class ScriptValidator : AbstractValidator<Script>
{
  public ScriptValidator()
  {
    RuleFor(x => x.Value).Script();
  }
}
