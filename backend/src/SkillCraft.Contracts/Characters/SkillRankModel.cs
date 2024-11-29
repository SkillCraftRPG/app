namespace SkillCraft.Contracts.Characters;

public record SkillRankModel
{
  public Skill Skill { get; set; }
  public int Rank { get; set; }

  public SkillRankModel() : this(default, default)
  {
  }

  public SkillRankModel(KeyValuePair<Skill, int> pair) : this(pair.Key, pair.Value)
  {
  }

  public SkillRankModel(Skill skill, int rank)
  {
    Skill = skill;
    Rank = rank;
  }
}
