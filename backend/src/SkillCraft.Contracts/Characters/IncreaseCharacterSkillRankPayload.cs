namespace SkillCraft.Contracts.Characters;

public record IncreaseCharacterSkillRankPayload
{
  public Skill Skill { get; set; }

  public IncreaseCharacterSkillRankPayload() : this(default(Skill))
  {
  }

  public IncreaseCharacterSkillRankPayload(Skill skill)
  {
    Skill = skill;
  }
}
