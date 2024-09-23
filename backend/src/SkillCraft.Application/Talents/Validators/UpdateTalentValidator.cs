using FluentValidation;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Talents.Validators;

internal class UpdateTalentValidator : AbstractValidator<UpdateTalentPayload>
{
  public UpdateTalentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.RequiredTalentId?.Value != null, () => RuleFor(x => x.RequiredTalentId!.Value!.Value).NotEmpty());
    When(x => x.Skill?.Value != null, () => RuleFor(x => x.Skill!.Value!.Value).IsInEnum());
  }
}
