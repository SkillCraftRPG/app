namespace SkillCraft.Contracts.Characters;

public class BonusModel
{
  public Guid Id { get; set; }

  public BonusCategory Category { get; set; }
  public string Target { get; set; }
  public int Value { get; set; }

  public bool IsTemporary { get; set; }
  public string? Precision { get; set; }
  public string? Notes { get; set; }

  public BonusModel() : this(default, string.Empty, default)
  {
  }

  public BonusModel(BonusCategory category, string target, int value)
  {
    Category = category;
    Target = target;
    Value = value;
  }

  public override bool Equals(object? obj) => obj is BonusModel bonus && bonus.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
