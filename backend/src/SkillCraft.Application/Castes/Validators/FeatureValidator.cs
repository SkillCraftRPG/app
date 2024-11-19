using FluentValidation;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Castes.Validators;

internal class FeatureValidator : AbstractValidator<FeaturePayload>
{
  public FeatureValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
