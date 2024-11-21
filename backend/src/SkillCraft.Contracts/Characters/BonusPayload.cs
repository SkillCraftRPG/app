namespace SkillCraft.Contracts.Characters;

public record BonusPayload
{
  public BonusCategory Category { get; set; }
  public string Target { get; set; }
  public int Value { get; set; }

  public bool IsTemporary { get; set; }
  public string? Precision { get; set; }
  public string? Notes { get; set; }

  public BonusPayload() : this(default, string.Empty, default)
  {
  }

  public BonusPayload(BonusCategory category, string target, int value)
  {
    Category = category;
    Target = target;
    Value = value;
  }
}
