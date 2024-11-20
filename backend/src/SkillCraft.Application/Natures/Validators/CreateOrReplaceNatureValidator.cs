using FluentValidation;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Natures.Validators;

internal class CreateOrReplaceNatureValidator : AbstractValidator<CreateOrReplaceNaturePayload>
{
  public CreateOrReplaceNatureValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Attribute.HasValue, () => RuleFor(x => x.Attribute!.Value).IsInEnum());
    When(x => x.GiftId.HasValue, () => RuleFor(x => x.GiftId!.Value).NotEmpty());
  }
}
