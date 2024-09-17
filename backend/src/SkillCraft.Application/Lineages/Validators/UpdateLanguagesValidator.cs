using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateLanguagesValidator : AbstractValidator<UpdateLanguagesPayload>
{
  public UpdateLanguagesValidator()
  {
    When(x => x.Ids != null, () => RuleForEach(x => x.Ids!).NotEmpty());
    When(x => x.Extra.HasValue, () => RuleFor(x => x.Extra!.Value).InclusiveBetween(0, 3));
    When(x => !string.IsNullOrWhiteSpace(x.Text?.Value), () => RuleFor(x => x.Text!.Value!).LanguagesText());
  }
}
