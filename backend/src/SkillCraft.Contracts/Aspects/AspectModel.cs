using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Aspects;

public class AspectModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributesModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public WorldModel World { get; set; }

  public AspectModel() : this(new WorldModel(), string.Empty)
  {
  }

  public AspectModel(WorldModel world, string name)
  {
    Name = name;

    Attributes = new();
    Skills = new();

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
