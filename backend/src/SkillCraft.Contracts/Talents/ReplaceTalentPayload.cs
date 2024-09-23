namespace SkillCraft.Contracts.Talents;

public record ReplaceTalentPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Guid? RequiredTalentId { get; set; }

  public ReplaceTalentPayload() : this(string.Empty)
  {
  }

  public ReplaceTalentPayload(string name)
  {
    Name = name;
  }
}
