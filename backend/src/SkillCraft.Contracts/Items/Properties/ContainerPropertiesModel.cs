namespace SkillCraft.Contracts.Items.Properties;

public record ContainerPropertiesModel : IContainerProperties
{
  public double? Capacity { get; set; } // TODO(fpion): nullability
  public double? Volume { get; set; } // TODO(fpion): nullability
}
