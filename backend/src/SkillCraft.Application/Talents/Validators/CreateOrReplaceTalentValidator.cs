﻿using FluentValidation;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Talents.Validators;

internal class CreateOrReplaceTalentValidator : AbstractValidator<CreateOrReplaceTalentPayload>
{
  public CreateOrReplaceTalentValidator()
  {
    RuleFor(x => x.Tier).Tier();

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.RequiredTalentId.HasValue, () => RuleFor(x => x.RequiredTalentId!.Value).NotEmpty());
  }
}
