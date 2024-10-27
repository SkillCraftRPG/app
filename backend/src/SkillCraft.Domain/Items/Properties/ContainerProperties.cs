using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record ContainerProperties : PropertiesBase, IContainerProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Container;

  public ContainerProperties(IContainerProperties container) : this()
  {
  }

  public ContainerProperties()
  {
    new ContainerPropertiesValidator().ValidateAndThrow(this);
  }
}
