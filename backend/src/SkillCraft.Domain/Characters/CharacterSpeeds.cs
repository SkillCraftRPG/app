using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Domain.Characters;

public record CharacterSpeeds : ISpeeds
{
  public int Walk { get; }
  public int Climb { get; }
  public int Swim { get; }
  public int Fly { get; }
  public int Hover { get; }
  public int Burrow { get; }

  public CharacterSpeeds(IEnumerable<KeyValuePair<SpeedKind, int>> speeds)
  {
    foreach (KeyValuePair<SpeedKind, int> speed in speeds)
    {
      switch (speed.Key)
      {
        case SpeedKind.Burrow:
          Burrow = speed.Value;
          break;
        case SpeedKind.Climb:
          Climb = speed.Value;
          break;
        case SpeedKind.Fly:
          Fly = speed.Value;
          break;
        case SpeedKind.Hover:
          Hover = speed.Value;
          break;
        case SpeedKind.Swim:
          Swim = speed.Value;
          break;
        case SpeedKind.Walk:
          Walk = speed.Value;
          break;
      }
    }
  }
}
