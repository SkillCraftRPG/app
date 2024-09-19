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

    RuleFor(x => x.Attributes).SetValidator(new AttributeBonusesValidator());
    RuleForEach(x => x.Features).SetValidator(new FeatureValidator());

    RuleFor(x => x.Languages).SetValidator(new LanguagesValidator());
    RuleFor(x => x.Names).SetValidator(new NamesValidator());

    RuleFor(x => x.Speeds).SetValidator(new SpeedsValidator());
    RuleFor(x => x.Size).SetValidator(new SizeValidator());
    RuleFor(x => x.Weight).SetValidator(new WeightValidator());
    RuleFor(x => x.Ages).SetValidator(new AgesValidator());
  }
}
