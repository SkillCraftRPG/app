﻿using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Domain.Characters;

public record CharacterSpeeds : ISpeeds
{
  public int Walk { get; }
  public int Climb { get; }
  public int Swim { get; }
  public int Fly { get; }
  public int Hover { get; }
  public int Burrow { get; }

  public CharacterSpeeds(Character character) : this(character.LineageSpeeds.Values, character.Bonuses.Values)
  {
  }

  public CharacterSpeeds(IEnumerable<Speeds> lineageSpeeds, IEnumerable<Bonus> bonuses)
  {
    Dictionary<SpeedKind, int> speeds = new()
    {
      [SpeedKind.Walk] = lineageSpeeds.Max(speed => speed.Walk),
      [SpeedKind.Climb] = lineageSpeeds.Max(speed => speed.Climb),
      [SpeedKind.Swim] = lineageSpeeds.Max(speed => speed.Swim),
      [SpeedKind.Fly] = lineageSpeeds.Max(speed => speed.Fly),
      [SpeedKind.Hover] = lineageSpeeds.Max(speed => speed.Hover),
      [SpeedKind.Burrow] = lineageSpeeds.Max(speed => speed.Burrow)
    };

    foreach (Bonus bonus in bonuses)
    {
      if (bonus.Category == BonusCategory.Speed && Enum.TryParse(bonus.Target, out SpeedKind speed))
      {
        speeds[speed] += bonus.Value;
      }
    }

    Walk = Math.Max(speeds[SpeedKind.Walk], 0);
    Climb = Math.Max(speeds[SpeedKind.Climb], 0);
    Swim = Math.Max(speeds[SpeedKind.Swim], 0);
    Fly = Math.Max(speeds[SpeedKind.Fly], 0);
    Hover = Math.Max(speeds[SpeedKind.Hover], 0);
    Burrow = Math.Max(speeds[SpeedKind.Burrow], 0);
  }
}
