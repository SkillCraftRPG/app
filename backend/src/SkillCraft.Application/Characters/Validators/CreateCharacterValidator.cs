﻿using FluentValidation;
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
    RuleForEach(x => x.LanguageIds).NotEmpty();

    RuleFor(x => x.NatureId).NotEmpty();
    RuleForEach(x => x.CustomizationIds).NotEmpty();

    RuleForEach(x => x.AspectIds).NotEmpty();
    RuleFor(x => x.AspectIds).Must(x => x.Distinct().Count() == 2)
      .WithErrorCode(nameof(CreateCharacterValidator))
      .WithMessage("'{PropertyName}' must contain exactly 2 different aspect identifiers.");

    RuleFor(x => x.Attributes).SetValidator(new BaseAttributesPayloadValidator());

    RuleFor(x => x.CasteId).NotEmpty();
    RuleFor(x => x.EducationId).NotEmpty();

    RuleForEach(x => x.TalentIds).NotEmpty();
    RuleFor(x => x.TalentIds).Must(x => x.Distinct().Count() == 2)
      .WithErrorCode(nameof(CreateCharacterValidator))
      .WithMessage("'{PropertyName}' must contain exactly 2 different aspect identifiers.");

    When(x => x.StartingWealth != null, () => RuleFor(x => x.StartingWealth!).SetValidator(new StartingWealthValidator()));
  }
}
