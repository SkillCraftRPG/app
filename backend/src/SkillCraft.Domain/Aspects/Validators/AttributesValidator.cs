using FluentValidation;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Domain.Aspects.Validators;

internal class AttributesValidator : AbstractValidator<IAttributes>
{
  public AttributesValidator()
  {
    When(x => x.Mandatory1.HasValue, () => RuleFor(x => x.Mandatory1!.Value).IsInEnum());
    When(x => x.Mandatory2.HasValue, () => RuleFor(x => x.Mandatory2!.Value).IsInEnum());
    When(x => x.Optional1.HasValue, () => RuleFor(x => x.Optional1!.Value).IsInEnum());
    When(x => x.Optional2.HasValue, () => RuleFor(x => x.Optional2!.Value).IsInEnum());

    // TODO(fpion): all attributes must differ
  }
}
