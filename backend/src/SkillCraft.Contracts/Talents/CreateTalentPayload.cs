namespace SkillCraft.Contracts.Talents;

public record CreateTalentPayload
{
  public int Tier { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Guid? RequiredTalentId { get; set; }
  public Skill? Skill { get; set; }

  public CreateTalentPayload() : this(string.Empty)
  {
  }

  public CreateTalentPayload(string name)
  {
    Name = name;
  }
}
