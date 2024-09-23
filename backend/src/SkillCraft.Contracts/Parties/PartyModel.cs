using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Parties;

public class PartyModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public WorldModel World { get; set; }

  public PartyModel() : this(new WorldModel(), string.Empty)
  {
  }

  public PartyModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
