using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateSizeValidator : AbstractValidator<UpdateSizePayload>
{
  public UpdateSizeValidator()
  {
    When(x => x.Category.HasValue, () => RuleFor(x => x.Category!.Value).IsInEnum());
    When(x => !string.IsNullOrWhiteSpace(x.Roll?.Value), () => RuleFor(x => x.Roll!.Value!).Roll());
  }
}
