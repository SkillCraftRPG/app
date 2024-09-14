using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class RollValidator : AbstractValidator<Roll>
{
  public RollValidator()
  {
    RuleFor(x => x.Value).Roll();
  }
}
