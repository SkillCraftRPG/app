using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class ReplaceLineageValidator : AbstractValidator<ReplaceLineagePayload>
{
  public ReplaceLineageValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Attributes).SetValidator(new AttributesValidator());
    RuleForEach(x => x.Traits).SetValidator(new TraitValidator());

    RuleFor(x => x.Languages).SetValidator(new LanguagesValidator());
  }
}
