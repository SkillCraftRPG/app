using FluentValidation;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Languages.Validators;

internal class ReplaceLanguageValidator : AbstractValidator<ReplaceLanguagePayload>
{
  public ReplaceLanguageValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Script), () => RuleFor(x => x.Script!).Script());
    When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers), () => RuleFor(x => x.TypicalSpeakers!).TypicalSpeakers());
  }
}
