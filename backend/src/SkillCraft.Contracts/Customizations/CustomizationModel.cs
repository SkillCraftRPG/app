using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Customizations;

public class CustomizationModel : Aggregate
{
  public WorldModel World { get; set; }

  public CustomizationType Type { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public CustomizationModel() : this(new WorldModel(), string.Empty)
  {
  }

  public CustomizationModel(WorldModel world, string name)
  {
    World = world;

    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
