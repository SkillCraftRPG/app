namespace SkillCraft.Contracts.Lineages;

public record SizeModel
{
  public SizeCategory Category { get; set; }
  public string Roll { get; set; }

  public SizeModel() : this(default, string.Empty)
  {
  }

  public SizeModel(SizeCategory category, string roll)
  {
    Category = category;
    Roll = roll;
  }
}
