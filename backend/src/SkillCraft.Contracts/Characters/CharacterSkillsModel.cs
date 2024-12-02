namespace SkillCraft.Contracts.Characters;

public record CharacterSkillsModel
{
  public CharacterSkillModel Acrobatics { get; set; } = new();
  public CharacterSkillModel Athletics { get; set; } = new();
  public CharacterSkillModel Craft { get; set; } = new();
  public CharacterSkillModel Deception { get; set; } = new();
  public CharacterSkillModel Diplomacy { get; set; } = new();
  public CharacterSkillModel Discipline { get; set; } = new();
  public CharacterSkillModel Insight { get; set; } = new();
  public CharacterSkillModel Investigation { get; set; } = new();
  public CharacterSkillModel Knowledge { get; set; } = new();
  public CharacterSkillModel Linguistics { get; set; } = new();
  public CharacterSkillModel Medicine { get; set; } = new();
  public CharacterSkillModel Melee { get; set; } = new();
  public CharacterSkillModel Occultism { get; set; } = new();
  public CharacterSkillModel Orientation { get; set; } = new();
  public CharacterSkillModel Perception { get; set; } = new();
  public CharacterSkillModel Performance { get; set; } = new();
  public CharacterSkillModel Resistance { get; set; } = new();
  public CharacterSkillModel Stealth { get; set; } = new();
  public CharacterSkillModel Survival { get; set; } = new();
  public CharacterSkillModel Thievery { get; set; } = new();

  public CharacterSkillsModel()
  {
  }

  public CharacterSkillsModel(CharacterModel character)
  {
    Dictionary<Skill, int> totals = new()
    {
      [Skill.Acrobatics] = character.Attributes.Agility.TemporaryModifier,
      [Skill.Melee] = character.Attributes.Agility.TemporaryModifier,
      [Skill.Stealth] = character.Attributes.Agility.TemporaryModifier,

      [Skill.Craft] = character.Attributes.Coordination.TemporaryModifier,
      [Skill.Orientation] = character.Attributes.Coordination.TemporaryModifier,
      [Skill.Thievery] = character.Attributes.Coordination.TemporaryModifier,

      [Skill.Investigation] = character.Attributes.Intellect.TemporaryModifier,
      [Skill.Knowledge] = character.Attributes.Intellect.TemporaryModifier,
      [Skill.Linguistics] = character.Attributes.Intellect.TemporaryModifier,

      [Skill.Deception] = character.Attributes.Presence.TemporaryModifier,
      [Skill.Diplomacy] = character.Attributes.Presence.TemporaryModifier,
      [Skill.Performance] = character.Attributes.Presence.TemporaryModifier,

      [Skill.Insight] = character.Attributes.Sensitivity.TemporaryModifier,
      [Skill.Medicine] = character.Attributes.Sensitivity.TemporaryModifier,
      [Skill.Perception] = character.Attributes.Sensitivity.TemporaryModifier,
      [Skill.Survival] = character.Attributes.Sensitivity.TemporaryModifier,

      [Skill.Discipline] = character.Attributes.Spirit.TemporaryModifier,
      [Skill.Occultism] = character.Attributes.Spirit.TemporaryModifier,

      [Skill.Athletics] = character.Attributes.Vigor.TemporaryModifier,
      [Skill.Resistance] = character.Attributes.Vigor.TemporaryModifier
    };

    HashSet<Skill> trainedSkills = character.Talents.Where(x => x.Talent.Skill.HasValue).Select(x => x.Talent.Skill!.Value).ToHashSet();
    foreach (SkillRankModel skillRank in character.SkillRanks)
    {
      totals[skillRank.Skill] += trainedSkills.Contains(skillRank.Skill) ? skillRank.Rank : skillRank.Rank / 2;
    }

    foreach (BonusModel bonus in character.Bonuses)
    {
      if (bonus.Category == BonusCategory.Skill && Enum.TryParse(bonus.Target, out Skill skill))
      {
        totals[skill] += bonus.Value;
      }
    }

    Acrobatics = new CharacterSkillModel(totals[Skill.Acrobatics], trainedSkills.Contains(Skill.Acrobatics));
    Athletics = new CharacterSkillModel(totals[Skill.Athletics], trainedSkills.Contains(Skill.Athletics));
    Craft = new CharacterSkillModel(totals[Skill.Craft], trainedSkills.Contains(Skill.Craft));
    Deception = new CharacterSkillModel(totals[Skill.Deception], trainedSkills.Contains(Skill.Deception));
    Diplomacy = new CharacterSkillModel(totals[Skill.Diplomacy], trainedSkills.Contains(Skill.Diplomacy));
    Discipline = new CharacterSkillModel(totals[Skill.Discipline], trainedSkills.Contains(Skill.Discipline));
    Insight = new CharacterSkillModel(totals[Skill.Insight], trainedSkills.Contains(Skill.Insight));
    Investigation = new CharacterSkillModel(totals[Skill.Investigation], trainedSkills.Contains(Skill.Investigation));
    Knowledge = new CharacterSkillModel(totals[Skill.Knowledge], trainedSkills.Contains(Skill.Knowledge));
    Linguistics = new CharacterSkillModel(totals[Skill.Linguistics], trainedSkills.Contains(Skill.Linguistics));
    Medicine = new CharacterSkillModel(totals[Skill.Medicine], trainedSkills.Contains(Skill.Medicine));
    Melee = new CharacterSkillModel(totals[Skill.Melee], trainedSkills.Contains(Skill.Melee));
    Occultism = new CharacterSkillModel(totals[Skill.Occultism], trainedSkills.Contains(Skill.Occultism));
    Orientation = new CharacterSkillModel(totals[Skill.Orientation], trainedSkills.Contains(Skill.Orientation));
    Perception = new CharacterSkillModel(totals[Skill.Perception], trainedSkills.Contains(Skill.Perception));
    Performance = new CharacterSkillModel(totals[Skill.Performance], trainedSkills.Contains(Skill.Performance));
    Resistance = new CharacterSkillModel(totals[Skill.Resistance], trainedSkills.Contains(Skill.Resistance));
    Stealth = new CharacterSkillModel(totals[Skill.Stealth], trainedSkills.Contains(Skill.Stealth));
    Survival = new CharacterSkillModel(totals[Skill.Survival], trainedSkills.Contains(Skill.Survival));
    Thievery = new CharacterSkillModel(totals[Skill.Thievery], trainedSkills.Contains(Skill.Thievery));
  }
}
