using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Lineages.Validators;

internal class LanguagesValidator : AbstractValidator<Languages>
{
  public LanguagesValidator()
  {
    RuleFor(x => x.Extra).InclusiveBetween(0, 3);
    When(x => x.Text != null, () => RuleFor(x => x.Text!).LanguagesText());
  }
}
