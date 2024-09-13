using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Castes;

public class CasteModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<TraitModel> Traits { get; set; }

  public WorldModel World { get; set; }

  public CasteModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CasteModel(WorldModel world, string name)
  {
    Name = name;

    Traits = [];

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
