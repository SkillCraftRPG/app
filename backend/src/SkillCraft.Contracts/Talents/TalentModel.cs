using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Talents;

public class TalentModel : Aggregate
{
  public WorldModel World { get; set; }

  public int Tier { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Skill? Skill { get; set; }

  public TalentModel? RequiredTalent { get; set; }
  public List<TalentModel> RequiringTalents { get; set; }

  public TalentModel() : this(new WorldModel(), string.Empty)
  {
  }

  public TalentModel(WorldModel world, string name)
  {
    World = world;

    Name = name;

    RequiringTalents = [];
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
