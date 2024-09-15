using FluentValidation;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Customizations.Validators;

internal class CreateCustomizationValidator : AbstractValidator<CreateCustomizationPayload>
{
  public CreateCustomizationValidator()
  {
    RuleFor(x => x.Type).IsInEnum();

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
