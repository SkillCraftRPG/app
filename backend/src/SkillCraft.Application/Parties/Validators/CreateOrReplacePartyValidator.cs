﻿using FluentValidation;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Parties.Validators;

internal class CreateOrReplacePartyValidator : AbstractValidator<CreateOrReplacePartyPayload>
{
  public CreateOrReplacePartyValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
