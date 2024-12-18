﻿using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class CreateOrReplaceLineageValidator : AbstractValidator<CreateOrReplaceLineagePayload>
{
  public CreateOrReplaceLineageValidator()
  {
    When(x => x.ParentId.HasValue, () => RuleFor(x => x.ParentId!.Value).NotEmpty());

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Attributes).SetValidator(new AttributeBonusesValidator());
    RuleForEach(x => x.Traits).SetValidator(new TraitValidator());

    RuleFor(x => x.Languages).SetValidator(new LanguagesValidator());
    RuleFor(x => x.Names).SetValidator(new NamesValidator());

    RuleFor(x => x.Speeds).SetValidator(new SpeedsValidator());
    RuleFor(x => x.Size).SetValidator(new SizeValidator());
    RuleFor(x => x.Weight).SetValidator(new WeightValidator());
    RuleFor(x => x.Ages).SetValidator(new AgesValidator());
  }
}
