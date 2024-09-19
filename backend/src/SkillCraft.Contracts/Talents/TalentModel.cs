using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Talents;

public class TalentModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }

  public WorldModel World { get; set; }

  public TalentModel() : this(new WorldModel(), string.Empty)
  {
  }

  public TalentModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
