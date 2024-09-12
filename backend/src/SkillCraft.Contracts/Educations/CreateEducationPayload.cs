namespace SkillCraft.Contracts.Educations;

public record CreateEducationPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public CreateEducationPayload() : this(string.Empty)
  {
  }

  public CreateEducationPayload(string name)
  {
    Name = name;
  }
}
