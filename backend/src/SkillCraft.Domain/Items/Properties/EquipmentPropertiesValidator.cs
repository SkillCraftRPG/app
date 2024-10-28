using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class EquipmentPropertiesValidator : AbstractValidator<IEquipmentProperties>
{
  public EquipmentPropertiesValidator()
  {
    RuleFor(x => x.Defense).GreaterThanOrEqualTo(0);
    When(x => x.Resistance != null, () => RuleFor(x => x.Resistance).GreaterThan(0));
    RuleForEach(x => x.Traits).IsInEnum();
  }
}
