namespace SkillCraft.Contracts.Educations;

public record CreateOrReplaceEducationPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public CreateOrReplaceEducationPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceEducationPayload(string name)
  {
    Name = name;
  }
}
