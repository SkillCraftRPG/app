namespace SkillCraft.Contracts.Items.Properties;

public record ConsumablePropertiesModel : IConsumableProperties
{
  public int? Charges { get; set; }
  public bool RemoveWhenEmpty { get; set; }
  public Guid? ReplaceWithItemWhenEmptyId { get; set; }
}
