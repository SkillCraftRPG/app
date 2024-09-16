using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Lineages.Validators;

internal class CreateLineageValidator : AbstractValidator<CreateLineagePayload>
{
  public CreateLineageValidator()
  {
    When(x => x.ParentId.HasValue, () => RuleFor(x => x.ParentId!.Value).NotEmpty());

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
