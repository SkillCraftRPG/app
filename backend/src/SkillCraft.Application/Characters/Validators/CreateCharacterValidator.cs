using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Characters.Validators;

internal class CreateCharacterValidator : AbstractValidator<CreateCharacterPayload>
{
  public CreateCharacterValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Player), () => RuleFor(x => x.Player!).Name());

    RuleFor(x => x.LineageId).NotEmpty();
    RuleFor(x => x.Height).GreaterThan(0.0);
    RuleFor(x => x.Weight).GreaterThan(0.0);
    RuleFor(x => x.Age).GreaterThan(0);

    RuleFor(x => x.PersonalityId).NotEmpty();
    RuleForEach(x => x.CustomizationIds).NotEmpty();

    RuleForEach(x => x.AspectIds).NotEmpty();
    RuleFor(x => x.AspectIds).Must(x => x.Distinct().Count() == 2)
      .WithErrorCode(nameof(CreateCharacterValidator))
      .WithMessage("'{PropertyName}' must contain exactly 2 different aspect identifiers.");

    RuleFor(x => x.Attributes).SetValidator(new BaseAttributesPayloadValidator());
  }
}
