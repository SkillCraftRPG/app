using FluentValidation;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Validators;

internal class NameCategoryValidator : AbstractValidator<NameCategory>
{
  public NameCategoryValidator()
  {
    RuleFor(x => x.Key).NotEmpty();
  }
}
