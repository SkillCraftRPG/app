using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record ContainerProperties : PropertiesBase, IContainerProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Container;

  public double? Capacity { get; }
  public double? Volume { get; }

  public ContainerProperties(IContainerProperties container) : this(container.Capacity, container.Volume)
  {
  }

  public ContainerProperties(double? capacity, double? volume)
  {
    Capacity = capacity;
    Volume = volume;
    new ContainerPropertiesValidator().ValidateAndThrow(this);
  }
}
