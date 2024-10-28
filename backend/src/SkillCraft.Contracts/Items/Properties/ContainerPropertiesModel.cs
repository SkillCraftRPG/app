namespace SkillCraft.Contracts.Items.Properties;

public record ContainerPropertiesModel : IContainerProperties
{
  public double? Capacity { get; set; }
  public double? Volume { get; set; }
}
