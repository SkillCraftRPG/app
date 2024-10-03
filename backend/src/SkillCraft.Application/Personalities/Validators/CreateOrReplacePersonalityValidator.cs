using FluentValidation;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Personalities.Validators;

internal class CreateOrReplacePersonalityValidator : AbstractValidator<CreateOrReplacePersonalityPayload>
{
  public CreateOrReplacePersonalityValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Attribute.HasValue, () => RuleFor(x => x.Attribute!.Value).IsInEnum());
    When(x => x.GiftId.HasValue, () => RuleFor(x => x.GiftId!.Value).NotEmpty());
  }
}
