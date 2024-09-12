using FluentValidation;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Worlds.Validators;

internal class UpdateWorldValidator : AbstractValidator<UpdateWorldPayload>
{
  public UpdateWorldValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Slug), () => RuleFor(x => x.Slug!).Slug());
    When(x => !string.IsNullOrWhiteSpace(x.Name?.Value), () => RuleFor(x => x.Name!.Value!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
