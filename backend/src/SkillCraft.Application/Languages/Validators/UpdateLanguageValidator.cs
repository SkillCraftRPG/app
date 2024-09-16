using FluentValidation;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Languages.Validators;

internal class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Script?.Value), () => RuleFor(x => x.Script!.Value!).Script());
    When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers?.Value), () => RuleFor(x => x.TypicalSpeakers!.Value!).TypicalSpeakers());
  }
}
