namespace SkillCraft.Contracts.Educations;

public record ReplaceEducationPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public ReplaceEducationPayload() : this(string.Empty)
  {
  }

  public ReplaceEducationPayload(string name)
  {
    Name = name;
  }
}
