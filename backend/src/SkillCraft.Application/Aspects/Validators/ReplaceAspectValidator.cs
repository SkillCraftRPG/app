using FluentValidation;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects.Validators;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Aspects.Validators;

internal class ReplaceAspectValidator : AbstractValidator<ReplaceAspectPayload>
{
  public ReplaceAspectValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Attributes).SetValidator(new AttributeSelectionValidator());
    RuleFor(x => x.Skills).SetValidator(new SkillsValidator());
  }
}
