using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class CreateLineageValidator : AbstractValidator<CreateLineagePayload>
{
  public CreateLineageValidator()
  {
    When(x => x.ParentId.HasValue, () => RuleFor(x => x.ParentId!.Value).NotEmpty());

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Attributes).SetValidator(new AttributesValidator());
    RuleForEach(x => x.Traits).SetValidator(new TraitValidator());

    RuleFor(x => x.Languages).SetValidator(new LanguagesValidator());
    // TODO(fpion): Names

    RuleFor(x => x.Speeds).SetValidator(new SpeedsValidator());
  }
}
