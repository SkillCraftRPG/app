using FluentValidation;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Castes.Validators;

internal class SaveCasteValidator : AbstractValidator<SaveCastePayload>
{
  public SaveCasteValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Skill.HasValue, () => RuleFor(x => x.Skill!.Value).IsInEnum());
    When(x => !string.IsNullOrWhiteSpace(x.WealthRoll), () => RuleFor(x => x.WealthRoll!).Roll());

    RuleForEach(x => x.Traits).SetValidator(new TraitValidator());
  }
}
