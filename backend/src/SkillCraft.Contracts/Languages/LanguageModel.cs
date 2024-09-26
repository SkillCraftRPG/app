using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Languages;

public class LanguageModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public string? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public LanguageModel() : this(new WorldModel(), string.Empty)
  {
  }

  public LanguageModel(WorldModel world, string name)
  {
    World = world;

    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
