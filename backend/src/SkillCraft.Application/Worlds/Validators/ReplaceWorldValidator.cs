using FluentValidation;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Worlds.Validators;

internal class ReplaceWorldValidator : AbstractValidator<ReplaceWorldPayload>
{
  public ReplaceWorldValidator()
  {
    RuleFor(x => x.Slug).Slug();
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
