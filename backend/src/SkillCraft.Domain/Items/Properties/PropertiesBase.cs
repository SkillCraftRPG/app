using SkillCraft.Contracts.Items;

namespace SkillCraft.Domain.Items.Properties;

public abstract record PropertiesBase
{
  [JsonIgnore]
  public abstract ItemCategory Category { get; }

  [JsonIgnore]
  public abstract int Size { get; }
}
