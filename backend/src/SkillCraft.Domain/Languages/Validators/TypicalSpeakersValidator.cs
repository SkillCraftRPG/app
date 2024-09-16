using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Languages.Validators;

internal class TypicalSpeakersValidator : AbstractValidator<TypicalSpeakers>
{
  public TypicalSpeakersValidator()
  {
    RuleFor(x => x.Value).TypicalSpeakers();
  }
}
