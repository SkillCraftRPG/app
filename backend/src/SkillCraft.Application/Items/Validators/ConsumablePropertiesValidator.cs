using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Application.Items.Validators;

internal class ConsumablePropertiesValidator : AbstractValidator<IConsumableProperties>
{
  public ConsumablePropertiesValidator()
  {
    When(x => x.Charges != null, () => RuleFor(x => x.Charges).GreaterThan(0));
    When(x => x.RemoveWhenEmpty, () => RuleFor(x => x.ReplaceWithItemWhenEmptyId).Null());
    When(x => x.ReplaceWithItemWhenEmptyId != null, () =>
    {
      RuleFor(x => x.RemoveWhenEmpty).NotEqual(true);
      RuleFor(x => x.ReplaceWithItemWhenEmptyId).NotEmpty();
    });
  }
}
