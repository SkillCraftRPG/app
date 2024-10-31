namespace SkillCraft.Contracts.Talents;

public record CreateOrReplaceTalentPayload
{
  public int Tier { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Guid? RequiredTalentId { get; set; }
  public Skill? Skill { get; set; }

  public CreateOrReplaceTalentPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceTalentPayload(string name)
  {
    Name = name;
  }
}
