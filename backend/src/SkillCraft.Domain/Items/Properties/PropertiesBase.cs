using SkillCraft.Contracts.Items;

namespace SkillCraft.Domain.Items.Properties;

public abstract record PropertiesBase
{
  public abstract ItemCategory Category { get; }
}
