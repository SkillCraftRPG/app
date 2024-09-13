namespace SkillCraft.Contracts.Educations;

public record UpdateEducationPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<Skill?>? Skill { get; set; }
  public Change<double?>? WealthMultiplier { get; set; }
}
