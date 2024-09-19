using FluentValidation;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects.Validators;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Aspects.Validators;

internal class UpdateAspectValidator : AbstractValidator<UpdateAspectPayload>
{
  public UpdateAspectValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Attributes != null, () => RuleFor(x => x.Attributes!).SetValidator(new AttributeSelectionValidator()));
    When(x => x.Skills != null, () => RuleFor(x => x.Skills!).SetValidator(new SkillsValidator()));
  }
}
