using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record ContainerProperties : PropertiesBase, IContainerProperties
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Container;

  public double? Capacity { get; }
  public double? Volume { get; }

  [JsonIgnore]
  public override int Size { get; } = 8 /* Capacity */ + 8 /* Volume */;

  public ContainerProperties(IContainerProperties container) : this(container.Capacity, container.Volume)
  {
  }

  [JsonConstructor]
  public ContainerProperties(double? capacity, double? volume)
  {
    Capacity = capacity;
    Volume = volume;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<ContainerProperties>
  {
    public Validator()
    {
      When(x => x.Capacity != null, () => RuleFor(x => x.Capacity).GreaterThan(0.0));
      When(x => x.Volume != null, () => RuleFor(x => x.Volume).GreaterThan(0.0));
    }
  }
}
