using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Personalities;

public class PersonalityModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public CustomizationModel? Gift { get; set; }

  public WorldModel World { get; set; }

  public PersonalityModel() : this(new WorldModel(), string.Empty)
  {
  }

  public PersonalityModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
