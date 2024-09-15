using FluentValidation;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Customizations.Validators;

internal class UpdateCustomizationValidator : AbstractValidator<UpdateCustomizationPayload>
{
  public UpdateCustomizationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
