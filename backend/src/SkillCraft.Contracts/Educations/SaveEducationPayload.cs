namespace SkillCraft.Contracts.Educations;

public record SaveEducationPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public SaveEducationPayload() : this(string.Empty)
  {
  }

  public SaveEducationPayload(string name)
  {
    Name = name;
  }
}
