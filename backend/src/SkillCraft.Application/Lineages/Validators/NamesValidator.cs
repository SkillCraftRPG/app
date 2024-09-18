using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class NamesValidator : AbstractValidator<NamesModel>
{
  public NamesValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Text), () => RuleFor(x => x.Text!).NamesText());
    RuleForEach(x => x.Custom).SetValidator(new NameCategoryValidator());
  }
}
