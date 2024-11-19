using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Castes;

public class CasteModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<FeatureModel> Features { get; set; }

  public CasteModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CasteModel(WorldModel world, string name)
  {
    World = world;

    Name = name;

    Features = [];
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
