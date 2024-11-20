using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Natures;

public class NatureModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public CustomizationModel? Gift { get; set; }

  public NatureModel() : this(new WorldModel(), string.Empty)
  {
  }

  public NatureModel(WorldModel world, string name)
  {
    World = world;

    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
