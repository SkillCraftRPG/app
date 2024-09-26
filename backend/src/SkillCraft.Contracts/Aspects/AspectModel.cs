using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Aspects;

public class AspectModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeSelectionModel Attributes { get; set; }
  public SkillsModel Skills { get; set; }

  public AspectModel() : this(new WorldModel(), string.Empty)
  {
  }

  public AspectModel(WorldModel world, string name)
  {
    World = world;

    Name = name;

    Attributes = new();
    Skills = new();
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
