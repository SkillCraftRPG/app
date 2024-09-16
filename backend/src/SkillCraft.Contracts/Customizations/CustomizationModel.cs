using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Customizations;

public class CustomizationModel : Aggregate
{
  public CustomizationType Type { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public WorldModel World { get; set; }

  public CustomizationModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CustomizationModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
