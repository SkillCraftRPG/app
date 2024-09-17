using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class LanguagesValidator : AbstractValidator<LanguagesPayload>
{
  public LanguagesValidator()
  {
    RuleForEach(x => x.Ids).NotEmpty();
    RuleFor(x => x.Extra).InclusiveBetween(0, 3);
    When(x => !string.IsNullOrWhiteSpace(x.Text), () => RuleFor(x => x.Text!).LanguagesText());
  }
}
