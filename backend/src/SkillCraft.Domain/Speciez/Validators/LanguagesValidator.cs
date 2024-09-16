using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain.Speciez.Validators;

internal class LanguagesValidator : AbstractValidator<Languages>
{
  public LanguagesValidator()
  {
    RuleFor(x => x.Extra).InclusiveBetween(0, 3);
    When(x => !string.IsNullOrWhiteSpace(x.Text), () => RuleFor(x => x.Text!).LanguagesText());
  }
}
