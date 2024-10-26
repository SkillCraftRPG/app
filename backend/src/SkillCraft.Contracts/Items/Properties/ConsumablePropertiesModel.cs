namespace SkillCraft.Contracts.Items.Properties;

public record ConsumablePropertiesModel : IConsumableProperties
{
  public int? Charges { get; set; } // TODO(fpion): nullability
}
