using FluentValidation;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Worlds.Validators;

internal class UpdateWorldValidator : AbstractValidator<UpdateWorldPayload>
{
  public UpdateWorldValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueSlug), () => RuleFor(x => x.UniqueSlug!).Slug());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
