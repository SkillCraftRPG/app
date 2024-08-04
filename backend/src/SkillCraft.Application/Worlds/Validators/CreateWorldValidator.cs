using FluentValidation;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Worlds.Validators;

internal class CreateWorldValidator : AbstractValidator<CreateWorldPayload>
{
  public CreateWorldValidator()
  {
    RuleFor(x => x.UniqueSlug).Slug();
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description != null, () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
