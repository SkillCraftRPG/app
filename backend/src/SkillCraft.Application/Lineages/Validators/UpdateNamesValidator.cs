using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class UpdateNamesValidator : AbstractValidator<UpdateNamesPayload>
{
  public UpdateNamesValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Text?.Value), () => RuleFor(x => x.Text!.Value!).NamesText());
    When(x => x.Custom != null, () => RuleForEach(x => x.Custom).SetValidator(new NameCategoryValidator()));
  }
}
