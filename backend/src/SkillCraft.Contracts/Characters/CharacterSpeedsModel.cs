using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Contracts.Characters;

public record CharacterSpeedsModel : ISpeeds
{
  public int Walk { get; set; }
  public int Climb { get; set; }
  public int Swim { get; set; }
  public int Fly { get; set; }
  public int Hover { get; set; }
  public int Burrow { get; set; }

  public CharacterSpeedsModel()
  {
  }

  public CharacterSpeedsModel(CharacterModel character)
  {
    Walk = Math.Max(character.Lineage.Speeds.Walk, character.Lineage.Species?.Speeds.Walk ?? 0);
    Climb = Math.Max(character.Lineage.Speeds.Climb, character.Lineage.Species?.Speeds.Climb ?? 0);
    Swim = Math.Max(character.Lineage.Speeds.Swim, character.Lineage.Species?.Speeds.Swim ?? 0);
    Fly = Math.Max(character.Lineage.Speeds.Fly, character.Lineage.Species?.Speeds.Fly ?? 0);
    Hover = Math.Max(character.Lineage.Speeds.Hover, character.Lineage.Species?.Speeds.Hover ?? 0);
    Burrow = Math.Max(character.Lineage.Speeds.Burrow, character.Lineage.Species?.Speeds.Burrow ?? 0);

    foreach (BonusModel bonus in character.Bonuses)
    {
      if (bonus.Category == BonusCategory.Speed && Enum.TryParse(bonus.Target, out SpeedKind speed))
      {
        switch (speed)
        {
          case SpeedKind.Burrow:
            Burrow += bonus.Value;
            break;
          case SpeedKind.Climb:
            Climb += bonus.Value;
            break;
          case SpeedKind.Fly:
            Fly += bonus.Value;
            break;
          case SpeedKind.Hover:
            Hover += bonus.Value;
            break;
          case SpeedKind.Swim:
            Swim += bonus.Value;
            break;
          case SpeedKind.Walk:
            Walk += bonus.Value;
            break;
        }
      }
    }
  }
}
