using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class SizeValidator : AbstractValidator<SizeModel>
{
  public SizeValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    When(x => !string.IsNullOrWhiteSpace(x.Roll), () => RuleFor(x => x.Roll!).Roll());
  }
}
