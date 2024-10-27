using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class ContainerPropertiesValidator : AbstractValidator<IContainerProperties>
{
  public ContainerPropertiesValidator()
  {
  }
}
