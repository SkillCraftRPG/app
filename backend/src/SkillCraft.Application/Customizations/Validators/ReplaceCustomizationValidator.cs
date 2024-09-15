using FluentValidation;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Customizations.Validators;

internal class ReplaceCustomizationValidator : AbstractValidator<ReplaceCustomizationPayload>
{
  public ReplaceCustomizationValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
