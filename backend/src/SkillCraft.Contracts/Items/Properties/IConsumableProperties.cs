namespace SkillCraft.Contracts.Items.Properties;

public interface IConsumableProperties
{
  int? Charges { get; }
  bool RemoveWhenEmpty { get; }
  Guid? ReplaceWithItemWhenEmptyId { get; }
}
