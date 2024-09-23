using FluentValidation;

namespace SkillCraft.Domain.Validators;

internal class EntityKeyValidator : AbstractValidator<EntityKey>
{
  public EntityKeyValidator()
  {
    RuleFor(x => x.Type).IsInEnum();
    RuleFor(x => x.Id).NotEmpty();
  }
}
