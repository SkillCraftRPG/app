using FluentValidation;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Educations.Validators;

internal class UpdateEducationValidator : AbstractValidator<UpdateEducationPayload>
{
  public UpdateEducationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Skill?.Value != null, () => RuleFor(x => x.Skill!.Value!.Value).IsInEnum());
    When(x => x.WealthMultiplier?.Value != null, () => RuleFor(x => x.WealthMultiplier!.Value!.Value).GreaterThan(0.0));
  }
}
