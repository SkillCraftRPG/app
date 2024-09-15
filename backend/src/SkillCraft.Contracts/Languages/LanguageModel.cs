using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Languages;

public class LanguageModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public string? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public WorldModel World { get; set; }

  public LanguageModel() : this(new WorldModel(), string.Empty)
  {
  }

  public LanguageModel(WorldModel world, string name)
  {
    Name = name;

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
