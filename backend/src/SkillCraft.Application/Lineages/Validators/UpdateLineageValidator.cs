using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateLineageValidator : AbstractValidator<UpdateLineagePayload>
{
  public UpdateLineageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Attributes != null, () => RuleFor(x => x.Attributes!).SetValidator(new UpdateAttributesValidator()));
    RuleForEach(x => x.Traits).SetValidator(new UpdateTraitValidator());

    When(x => x.Languages != null, () => RuleFor(x => x.Languages!).SetValidator(new UpdateLanguagesValidator()));
    When(x => x.Names != null, () => RuleFor(x => x.Names).SetValidator(new UpdateNamesValidator()));

    RuleFor(x => x.Speeds).SetValidator(new UpdateSpeedsValidator());
    RuleFor(x => x.Size).SetValidator(new UpdateSizeValidator());
    RuleFor(x => x.Weight).SetValidator(new UpdateWeightValidator());
    RuleFor(x => x.Ages).SetValidator(new UpdateAgesValidator());
  }
}
