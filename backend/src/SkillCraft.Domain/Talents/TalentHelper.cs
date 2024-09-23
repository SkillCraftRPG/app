using SkillCraft.Contracts;

namespace SkillCraft.Domain.Talents;

internal static class TalentHelper
{
  private static readonly Dictionary<string, Skill> _french = new()
  {
    ["Acrobaties"] = Skill.Acrobatics,
    ["Artisanat"] = Skill.Craft,
    ["Athlétisme"] = Skill.Athletics,
    ["Connaissance"] = Skill.Knowledge,
    ["Diplomatie"] = Skill.Diplomacy,
    ["Discipline"] = Skill.Discipline,
    ["Furtivité"] = Skill.Stealth,
    ["Intuition"] = Skill.Insight,
    ["Investigation"] = Skill.Investigation,
    ["Linguistique"] = Skill.Linguistics,
    ["Médecine"] = Skill.Medicine,
    ["Mêlée"] = Skill.Melee,
    ["Occultisme"] = Skill.Occultism,
    ["Orientation"] = Skill.Orientation,
    ["Perception"] = Skill.Perception,
    ["Performance"] = Skill.Performance,
    ["Résistance"] = Skill.Resistance,
    ["Roublardise"] = Skill.Thievery,
    ["Survie"] = Skill.Survival,
    ["Tromperie"] = Skill.Deception
  };

  public static Skill? TryGetSkill(Name name)
  {
    if (_french.TryGetValue(name.Value, out Skill skill))
    {
      return skill;
    }
    else if (Enum.TryParse(name.Value, out skill))
    {
      return skill;
    }

    return null;
  }
}
