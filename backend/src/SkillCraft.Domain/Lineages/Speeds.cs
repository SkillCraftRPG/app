using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Speeds : ISpeeds
{
  public int Walk { get; }
  public int Climb { get; }
  public int Swim { get; }
  public int Fly { get; }
  public int Hover { get; }
  public int Burrow { get; }

  public Speeds()
    : this(walk: 0, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0)
  {
  }

  public Speeds(ISpeeds speeds)
    : this(speeds.Walk, speeds.Climb, speeds.Swim, speeds.Fly, speeds.Hover, speeds.Burrow)
  {
  }

  [JsonConstructor]
  public Speeds(int walk, int climb, int swim, int fly, int hover, int burrow)
  {
    Walk = walk;
    Climb = climb;
    Swim = swim;
    Fly = fly;
    Hover = hover;
    Burrow = burrow;
    new SpeedsValidator().ValidateAndThrow(this);
  }
}
