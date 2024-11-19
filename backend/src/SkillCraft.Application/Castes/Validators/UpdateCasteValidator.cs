using FluentValidation;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Castes.Validators;

internal class UpdateCasteValidator : AbstractValidator<UpdateCastePayload>
{
  public UpdateCasteValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Skill?.Value != null, () => RuleFor(x => x.Skill!.Value).IsInEnum());
    When(x => !string.IsNullOrWhiteSpace(x.WealthRoll?.Value), () => RuleFor(x => x.WealthRoll!.Value!).Roll());

    RuleForEach(x => x.Features).SetValidator(new UpdateFeatureValidator());
  }
}
