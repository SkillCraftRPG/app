using FluentValidation;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Natures.Validators;

internal class UpdateNatureValidator : AbstractValidator<UpdateNaturePayload>
{
  public UpdateNatureValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Attribute?.Value != null, () => RuleFor(x => x.Attribute!.Value!.Value).IsInEnum());
    When(x => x.GiftId?.Value != null, () => RuleFor(x => x.GiftId!.Value!.Value).NotEmpty());
  }
}
