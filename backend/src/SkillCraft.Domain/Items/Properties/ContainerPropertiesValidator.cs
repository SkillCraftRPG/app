using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class ContainerPropertiesValidator : AbstractValidator<IContainerProperties>
{
  public ContainerPropertiesValidator()
  {
    When(x => x.Capacity != null, () => RuleFor(x => x.Capacity).GreaterThan(0.0));
    When(x => x.Volume != null, () => RuleFor(x => x.Volume).GreaterThan(0.0));
  }
}
