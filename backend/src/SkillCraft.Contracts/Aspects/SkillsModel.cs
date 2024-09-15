namespace SkillCraft.Contracts.Aspects;

public record SkillsModel : ISkills
{
  public Skill? Discounted1 { get; set; }
  public Skill? Discounted2 { get; set; }
}
