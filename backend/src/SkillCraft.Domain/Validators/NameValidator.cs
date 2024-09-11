using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class NameValidator : AbstractValidator<Name>
{
  public NameValidator()
  {
    RuleFor(x => x.Value).Name();
  }
}
